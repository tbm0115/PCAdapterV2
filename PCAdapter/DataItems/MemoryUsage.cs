using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AdapterContext;


namespace PCAdapter
{
  public class MemoryUsage : ADataItem
  {
    public new string Name => "Memory Usage";
    public new bool IsActive { get; set; }
    public new int TickFrequency { get; set; }
    private decimal totalMemory { get; set; }

    public MemoryUsage():base()
    {
      this.totalMemory = (decimal)PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
    }
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
      Int64 tot = PerformanceInfo.GetTotalMemoryInMiB();
      decimal percFree = (this.totalMemory / (decimal)tot) * 100;
      decimal percOcc = 100 - percFree;
      base.Value = percOcc;// (object)new PerformanceCounter("Memory", "Available MBytes").NextValue();
    }
  }
}