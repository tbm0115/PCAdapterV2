﻿using System.Diagnostics;
using System.Threading.Tasks;
using AdapterContext;


namespace PCAdapter
{
  public class CPUUsage : ADataItem
  {
    public new string Name => "CPU Usage";
    public new bool IsActive { get; set; }
    public new int TickFrequency { get; set; }
    private bool isTicking { get; set; }

        private PerformanceCounter pc;
        public CPUUsage()
        {
            pc = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        }

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
      base.Value = pc.NextValue();
    }
  }
}