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
        private decimal totalMemory;
        private PerformanceCounter pc;

    public MemoryUsage():base()
    {
      this.totalMemory = (decimal)PerformanceInfo.GetPhysicalAvailableMemoryInMiB();
            this.pc = new PerformanceCounter("Memory", "Available MBytes");
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
            var val = pc.NextValue();
            //long tot = PerformanceInfo.GetTotalMemoryInMiB();
      decimal percFree = ((decimal)val / this.totalMemory) * 100;
      decimal percOcc = 100 - percFree;
      base.Value = percOcc;// (object)new PerformanceCounter("Memory", "Available MBytes").NextValue();
    }
  }
}