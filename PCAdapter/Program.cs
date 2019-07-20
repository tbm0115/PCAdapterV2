using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime;
using System.Collections.Generic;

namespace PCAdapter
{
    class Program
    {
        public enum ViewMode
        {
            Log,
            Table
        }
        public static ViewMode View;
        public static string[] pollRateOptions =
        {
      "100",
      "200",
      "300",
      "400",
      "500",
      "750",
      "1000",
      "1250",
      "1500",
      "2000",
      "5000",
      "10000"
    };
        static void Main(string[] args)
        {
            Console.Title = "PC Adapter";

            var ctx = new AdapterContext.AdapterModel();
            ctx.Database.CreateIfNotExists();

            bool skipPrompts = false;

            string adapterName = string.Empty;
            string defaultView = "0";
            if (args.Length > 0)
            {
                string tmpName = args[0];
                if (ctx.Adapters.Any(o => o.Name.ToLower() == tmpName.ToLower()))
                {
                    adapterName = args[0];
                    skipPrompts = true;
                }
                if (args.Length > 1)
                {
                    defaultView = args[1];
                }
            }

            if (string.IsNullOrEmpty(adapterName))
            {
                adapterName = SelectAdapterConfig(ctx);
            }

            using (var _adapter = new Adapter(ctx, adapterName))
            {
                _adapter.Updated += Adapter_Updated;

                if (!skipPrompts)
                {
                    if (ask($"Would you like to configure Adapter '{_adapter.Config.Name}'?", true, true))
                    {
                        if (ask($"Would you like to change the Polling Rate for Adapter '{_adapter.Config.Name}'? Current Poll Rate is {_adapter.Config.PollRate}ms", true, true))
                        {
                            int nwAdapterPollRate = PromptForPollRate();
                            _adapter.Config.PollRate = nwAdapterPollRate;
                        }
                    }

                    if (ask("Would you like to configure DataItems before we begin?", true, true))
                    {
                        ConfigureDataItems(_adapter);
                    }
                }

                if (skipPrompts || ask("Are you ready to start monitoring?"))
                {
                    Monitor(_adapter, defaultView);
                }
            }

            write("Press enter to exit");
            Console.ReadLine();
        }
        private static string SelectAdapterConfig(AdapterContext.AdapterModel ctx)
        {
            string selectedAdapterName = string.Empty;
            List<string> adapterNames = ctx.Adapters.Select(o => o.Name).ToList();
            adapterNames.Add("** Create a new Config **");
            if (adapterNames.Count == 1)
            {
                write("No Adapters available, please create a new one.", ConsoleColor.Red);
                selectedAdapterName = CreateNewAdapterConfig(ctx);
            }
            else
            {
                int selectedAdapterIndex = prompt("Choose an Adapter Config to use:", true, adapterNames.ToArray());
                if (selectedAdapterIndex == adapterNames.Count - 1)
                {
                    selectedAdapterName = CreateNewAdapterConfig(ctx);
                }
                else
                {
                    selectedAdapterName = adapterNames[selectedAdapterIndex];
                }
            }
            return selectedAdapterName;
        }
        private static string CreateNewAdapterConfig(AdapterContext.AdapterModel ctx)
        {
            bool adapterValid = false;
            string nwAdapterName = string.Empty;
            do
            {
                if (!string.IsNullOrEmpty(nwAdapterName))
                {
                    write("Adapter name already used!", ConsoleColor.Red);
                }
                nwAdapterName = input("Enter a new adapter name:", ConsoleColor.Yellow);
                if (string.IsNullOrEmpty(nwAdapterName) || ctx.Adapters.Any(o => o.Name.ToLower() == nwAdapterName.ToLower()))
                {
                    adapterValid = false;
                }
                else
                {
                    adapterValid = true;
                }
            } while (adapterValid == false);
            int nwAdapterPollRate = PromptForPollRate();
            var nwAdapter = new AdapterContext.DataModel.AdapterConfig()
            {
                Name = nwAdapterName,
                PollRate = nwAdapterPollRate,
                Created = DateTime.UtcNow,
                LastStarted = DateTime.UtcNow
            };
            ctx.Adapters.Add(nwAdapter);
            ctx.SaveChanges();

            return nwAdapter.Name;
        }

        private static int PromptForPollRate()
        {
            int nwAdapterPollRateSelection = prompt("Choose a poll rate option (in milliseconds):", true, pollRateOptions);
            int nwAdapterPollRate;
            int.TryParse(pollRateOptions[nwAdapterPollRateSelection], out nwAdapterPollRate);
            return nwAdapterPollRate;
        }

        private static void Monitor(Adapter _adapter, string defaultView = "0")
        {
            _adapter.Start();
            string input = defaultView;
            View = ViewMode.Log;
            ResetView(-101);
            do
            {
                if (Console.KeyAvailable)
                {
                    input = Console.ReadKey().KeyChar.ToString();
                    if (string.IsNullOrEmpty(input))
                    {
                        input = "EXIT";
                    }
                }
                switch (input)
                {
                    case "0":
                        View = ViewMode.Log;
                        input = string.Empty;
                        ResetView();
                        break;
                    case "1":
                        View = ViewMode.Table;
                        input = string.Empty;
                        ResetView();
                        break;
                    case "EXIT":
                        break;
                    default:
                        break;
                }

                System.Threading.Thread.Sleep(1000 / 60); // 60fps
            } while (string.IsNullOrEmpty(input));
            _adapter.Stop();
            //UpdateDataItems();
        }

        private static void ConfigureDataItems(Adapter _adapter)
        {
            int dataItemSelection = -1;
            do
            {
                List<string> opts = new List<string>();
                foreach (var item in _adapter.DataItems.Values)
                {
                    opts.Add(item.Name + "\t" + (item.IsActive ? "Active" : "Inactive"));
                }
                opts.Add("Finished");
                dataItemSelection = prompt("Choose a DataItem to change: ", options: opts.ToArray());
                if (dataItemSelection >= 0 && dataItemSelection < _adapter.DataItems.Count)
                {
                    var diKey = _adapter.DataItems.Keys.ToArray()[dataItemSelection];
                    var di = _adapter.DataItems[diKey];
                    if (ask($"Do you wish to {(di.IsActive ? "Disable" : "Enable")} {di.Name}?"))
                    {
                        _adapter.DataItems[diKey].IsActive = !di.IsActive;
                    }
                    dataItemSelection = -1;
                }

            } while (dataItemSelection <= 0);
            if (ask("Would you like to save this configuration?"))
            {
                if (!_adapter.SaveConfig())
                {
                    write("An error occurred while trying to save the Adapter Config.");
                }
            }
        }

        private static void ResetView(int? resetTick = null)
        {
            Console.Clear();
            write("Press enter to exit at anytime");
            write("Change View: (0=Log, 1=Real-Time Table)");
            //write(input, ConsoleColor.Cyan);
            if (resetTick != null)
            {
                lastResetTick = (int)resetTick;
            }
        }
        public static int lastResetTick = 0;
        private static void Adapter_Updated(Adapter sender, Adapter.AdapterUpdatedEventArguments e)
        {
            //UpdateDataItems(e.Changes);
            if ((e.TickIndex - lastResetTick) > 100)
            {
                ResetView(e.TickIndex);
            }
            switch (View)
            {
                case ViewMode.Log:
                    write($"{e.Timestamp.ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss")}\t{e.Changes.Count} changes");
                    break;
                case ViewMode.Table:
                    foreach (var item in e.Changes)
                    {
                        write($"{item.Timestamp.ToLocalTime().ToString("MM/dd/yyyy hh:mm:ss")}\t{item.Name}\t{item.Value.ToString()}");
                    }
                    break;
                default:
                    break;
            }
        }
        //private static void UpdateDataItems(List<Adapter.ChangedItem> changedItems, bool closeOnly = false)
        //{
        //    foreach (var item in changedItems)
        //    {
        //        var lastItems = ctx.Durations.Include("Item").Where(o => o.Item.Name == item.Name && o.Ended == null).OrderByDescending(o => o.Started).ToList();
        //        if (lastItems != null)
        //        {
        //            foreach (var lastItem in lastItems)
        //            {
        //                lastItem.Ended = item.Timestamp;
        //                lastItem.Timespan = ((DateTime)lastItem.Ended - lastItem.Started).Ticks;
        //                ctx.Entry(lastItem).State = System.Data.Entity.EntityState.Modified;
        //            }
        //        }
        //        if (!closeOnly)
        //        {
        //            var relItem = ctx.DataItems.FirstOrDefault(o => o.Name == item.Name);
        //            if (relItem != null)
        //            {
        //                var nwDur = new AdapterContext.DataModel.Duration()
        //                {
        //                    Item = relItem,
        //                    Started = item.Timestamp,
        //                    Value = item.Value.ToString(),
        //                    Ended = null,
        //                    Timespan = null
        //                };
        //                ctx.Durations.Add(nwDur);
        //                ctx.Entry(nwDur).State = System.Data.Entity.EntityState.Added;
        //            }
        //        }
        //    }
        //    ctx.SaveChanges();
        //}
        //private static void UpdateDataItems()
        //{
        //    UpdateDataItems(_adapter.Changed.ToList()); // Add any pending changed items
        //    List<Adapter.ChangedItem> lingers = new List<Adapter.ChangedItem>();
        //    foreach (KeyValuePair<Type, dynamic> kvp in _adapter.DataItems)
        //    {
        //        string itemName = kvp.Value.Name;
        //        string itemValue = kvp.Value.Value.ToString();
        //        if (ctx.Durations.Any(o => o.Item.Name == itemName && o.Ended == null && o.Value == itemValue))
        //        {
        //            lingers.Add(new Adapter.ChangedItem(new AdapterContext.DataItemChangedEventArguments(kvp.Value)));
        //        }
        //    }
        //    if (lingers != null && lingers.Count > 0)
        //    {
        //        UpdateDataItems(lingers, true);
        //    }
        //}

        public static string input(string message, ConsoleColor color = ConsoleColor.White, bool allowEmpty = false)
        {
            string output = string.Empty;
            bool valid = false;
            do
            {
                //Console.Clear();
                write(message, color);
                output = Console.ReadLine();
                if (allowEmpty)
                {
                    valid = true;
                }
                else if (!string.IsNullOrEmpty(output))
                {
                    valid = true;
                }
            } while (!valid);
            return output;
        }
        public static void write(string message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }
        public static bool ask(string message, bool clear = false, bool allowEmpty = false)
        {
            string input = "";
            do
            {
                if (clear)
                {
                    Console.Clear();
                }
                write(message, ConsoleColor.Yellow);
                write("(Y=Yes, N" + (allowEmpty ? " or Press Enter" : "") + "=No)", ConsoleColor.Gray);
                input = Console.ReadLine();
                if (input.ToLower() != "y" && input.ToLower() != "n" && !string.IsNullOrEmpty(input))
                {
                    write("Invalid input!", ConsoleColor.Red);
                }
            } while ((allowEmpty ? false : string.IsNullOrEmpty(input)) && input.ToLower() != "y" && input.ToLower() != "n");
            return input.ToLower() == "y";
        }
        public static int prompt(string message, bool clear = false, params string[] options)
        {
            string input = "";
            int selection = -1;
            do
            {
                if (clear)
                {
                    Console.Clear();
                }
                write(message, ConsoleColor.Yellow);
                write("Choose the corresponding number from the options below:", ConsoleColor.Gray);
                for (int i = 0; i < options.Length; i++)
                {
                    write((i + 1).ToString() + ") " + options[i], ConsoleColor.DarkYellow);
                }

                input = Console.ReadLine();
                Int32.TryParse(input, out selection);
                if (selection <= 0 || selection > (options.Length + 1))
                {
                    write("Invalid selection!", ConsoleColor.Red);
                    selection = -1;
                }
                else
                {
                    selection--;
                }
            } while (selection < 0);
            return selection;
        }
    }
}
