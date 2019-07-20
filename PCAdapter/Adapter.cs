using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Reflection;
using System.Timers;
using AdapterContext;
using AdapterContext.DataModel;

namespace PCAdapter
{

    public sealed class Adapter:IDisposable
    {
        public Dictionary<Type, dynamic> DataItems { get; set; }
        private Timer timer { get; set; }
        public int Frequence => this.Config.PollRate;
        public event UpdateHandler Updated;
        public delegate void UpdateHandler(Adapter sender, AdapterUpdatedEventArguments e);
        public HashSet<ChangedItem> Changed { get; set; }
        private int tickCount { get; set; }
        private AdapterModel _ctx { get; set; }
        public AdapterConfig Config { get; set; }

        public Adapter(AdapterModel ctx, string configName)
        {
            this._ctx = ctx;
            this.Init(configName);

            this.timer = new Timer(this.Config.PollRate);

        }
        public void Start()
        {
            this.timer.Start();
            this.timer.Elapsed += tick;
        }
        public void Stop()
        {
            this.timer.Stop();
            this.timer.Elapsed -= tick;
            System.Threading.Thread.Sleep(this.Config.PollRate * 2);
        }
        private void tick(object sender, ElapsedEventArgs e)
        {
            this.tickCount++;
            foreach (KeyValuePair<Type, dynamic> kvp in this.DataItems.Where(o => o.Value.IsActive))
            {
                kvp.Value.Tick();
            }
            ChangedItem[] changes;
            lock (this.Changed)
            {
                changes = this.Changed.ToArray();
                this.Changed.Clear();
            }
            if (changes.Length > 0)
            {
                this.SaveUpdates(changes);
                this.Updated(this, new AdapterUpdatedEventArguments(this.tickCount, changes));
            }
        }
        public void DataItemChanged(object sender, DataItemChangedEventArguments e)
        {
            lock (this.Changed)
            {
                this.Changed.Add(new ChangedItem(e));
            }
        }
        public void Init(string configName)
        {
            this.Changed = new HashSet<ChangedItem>();
            this.DataItems = this.FindImplementations();

            this.Config = this._ctx.Adapters.FirstOrDefault(o => o.Name == configName);
            if (this.Config == null)
            {
                this.Config = new AdapterConfig();
                this.Config.Created = DateTime.UtcNow;
                this.Config.PollRate = 500;
                this._ctx.Adapters.Add(this.Config);
            }
            else
            {
                this._ctx.Entry(this.Config).State = System.Data.Entity.EntityState.Modified;
            }
            this.Config.Name = configName;
            this.Config.LastStarted = DateTime.UtcNow;

            foreach (KeyValuePair<Type, dynamic> kvp in this.DataItems)
            {
                (kvp.Value as ADataItem).DataItemChanged += DataItemChanged;
                var configItem = this.Config.Items.FirstOrDefault(o => o.Name == kvp.Value.Name);
                if (configItem == null)
                {
                    configItem = new DataItem();
                    this.Config.Items.Add(configItem);
                    this._ctx.Entry(configItem).State = System.Data.Entity.EntityState.Added;
                    configItem.Name = kvp.Value.Name;
                    configItem.IsActive = kvp.Value.IsActive;
                    configItem.TickFrequency = kvp.Value.TickFrequency;
                }
                else
                {
                    kvp.Value.IsActive = configItem.IsActive;
                    kvp.Value.TickFrequency = configItem.TickFrequency;
                }
            }
            this._ctx.SaveChanges();
        }

        private void SaveUpdates(ChangedItem[] changes, bool closeOnly = false)
        {
            foreach (var item in changes)
            {
                var lastItems = this._ctx.Durations.Include("Item").Where(o => o.Item.Name == item.Name && o.Ended == null).OrderByDescending(o => o.Started).ToList();
                if (lastItems != null)
                {
                    foreach (var lastItem in lastItems)
                    {
                        lastItem.Ended = item.Timestamp;
                        lastItem.Timespan = ((DateTime)lastItem.Ended - lastItem.Started).Ticks;
                        this._ctx.Entry(lastItem).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                if (!closeOnly)
                {
                    var relItem = this._ctx.DataItems.FirstOrDefault(o => o.Name == item.Name);
                    if (relItem != null)
                    {
                        var nwDur = new AdapterContext.DataModel.Duration()
                        {
                            Item = relItem,
                            Started = item.Timestamp,
                            Value = item.Value.ToString(),
                            Ended = null,
                            Timespan = null
                        };
                        this._ctx.Durations.Add(nwDur);
                        this._ctx.Entry(nwDur).State = System.Data.Entity.EntityState.Added;
                    }
                }
            }
            this._ctx.SaveChanges();

        }

        private Dictionary<Type, dynamic> FindImplementations()
        {
            var dataItems = new Dictionary<Type, dynamic>();

            Assembly assembly = Assembly.GetCallingAssembly();
            Type diType = typeof(IDataItem);
            Type[] types = assembly.GetTypes().Where(o => !o.IsAbstract && o.GetInterfaces().Any(a => a.FullName == diType.FullName)).ToArray();
            foreach (var type in types)
            {
                var obj = Activator.CreateInstance(type);
                if (obj != null && !dataItems.ContainsKey(type))
                {
                    dataItems.Add(type, obj);
                }
            }
            return dataItems;
        }

        public class ChangedItem
        {
            public object Value { get; set; }
            public DateTime Timestamp { get; set; }
            public string Name { get; set; }

            public ChangedItem(DataItemChangedEventArguments obj)
            {
                this.Value = obj.DataItem.Value;
                this.Timestamp = obj.Timestamp;
                this.Name = obj.DataItem.Name;
            }
        }

        public class AdapterUpdatedEventArguments
        {
            public DateTime Timestamp { get; set; }
            public List<ChangedItem> Changes { get; set; }
            public int TickIndex { get; set; }

            public AdapterUpdatedEventArguments(int index, ChangedItem[] changes)
            {
                this.TickIndex = index;
                this.Timestamp = DateTime.UtcNow;
                this.Changes = new List<ChangedItem>(changes);
            }
        }

        //private XmlDocument ToXML()
        //{
        //  XmlDocument xDoc = new XmlDocument();
        //  xDoc.AppendChild(xDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
        //  XmlNode xDataItems = xDoc.AppendChild(xDoc.CreateElement("DataItems"));
        //  foreach (KeyValuePair<Type,dynamic> kvp in this.DataItems)
        //  {
        //    XmlNode xItem = kvp.Value.ToXML(xDoc);
        //    xItem.SelectSingleNode("Name").InnerText = kvp.Value.Name;
        //    xItem.SelectSingleNode("IsActive").InnerText = kvp.Value.IsActive.ToString();
        //    xItem.SelectSingleNode("TickFrequency").InnerText = kvp.Value.TickFrequency.ToString();
        //    xDataItems.AppendChild(xItem);
        //  }
        //  return xDoc;
        //}
        public bool SaveConfig(string pathOverride = null)
        {
            try
            {
                foreach (KeyValuePair<Type, dynamic> kvp in this.DataItems)
                {
                    (kvp.Value as ADataItem).DataItemChanged += DataItemChanged;
                    var configItem = this.Config.Items.FirstOrDefault(o => o.Name == kvp.Value.Name);
                    if (configItem == null)
                    {
                        configItem = new DataItem();
                        this.Config.Items.Add(configItem);
                        this._ctx.Entry(configItem).State = System.Data.Entity.EntityState.Added;
                        configItem.Name = kvp.Value.Name;
                    }
                    else
                    {
                        this._ctx.Entry(configItem).State = System.Data.Entity.EntityState.Modified;
                    }
                    configItem.IsActive = kvp.Value.IsActive;
                    configItem.TickFrequency = kvp.Value.TickFrequency;
                }
                this._ctx.Entry(this.Config).State = System.Data.Entity.EntityState.Modified;
                this._ctx.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public void Dispose()
        {
            this.Stop();
            this.SaveUpdates(this.Changed.ToArray());

            List<Adapter.ChangedItem> lingers = new List<Adapter.ChangedItem>();
            foreach (KeyValuePair<Type, dynamic> kvp in this.DataItems)
            {
                string itemName = kvp.Value.Name;
                string itemValue = kvp.Value.Value.ToString();
                if (this._ctx.Durations.Any(o => o.Item.Name == itemName && o.Ended == null && o.Value == itemValue))
                {
                    lingers.Add(new Adapter.ChangedItem(new AdapterContext.DataItemChangedEventArguments(kvp.Value)));
                }
            }
            if (lingers != null && lingers.Count > 0)
            {
                this.SaveUpdates(lingers.ToArray(), true);
            }

            this._ctx.Dispose();
        }
    }
}
