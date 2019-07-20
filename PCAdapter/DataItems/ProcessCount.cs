using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class ProcessCount : ADataItem
  {
    public new string Name => "Process Count";
    public new bool IsActive { get; set; }
    public new int TickFrequency { get; set; }
    private bool isTicking { get; set; }
    public override async void Tick()
    {
      if (!this.isTicking)
      {
        this.isTicking = true;
        await Task.Run(tick);
        this.isTicking = false;
      }
    }
    private void tick()
    {
      try
      {
        base.Value = (object)Process.GetProcesses().Length;
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}