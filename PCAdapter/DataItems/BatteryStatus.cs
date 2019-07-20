using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class BatteryStatus : ADataItem
  {
    public new string Name => "Battery Status";
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
        WindowHandles.SystemPower.SystemPowerStatus sps;
        if (WindowHandles.SystemPower.GetSystemPowerStatus(out sps))
        {
          WindowHandles.SystemPower.BatteryFlag flgBattery = sps.flgBattery;
          if ((uint)flgBattery <= 8U)
          {
            switch (flgBattery)
            {
              case WindowHandles.SystemPower.BatteryFlag.High:
                base.Value = (object)"HIGH";
                break;
              case WindowHandles.SystemPower.BatteryFlag.Low:
                base.Value = (object)"LOW";
                break;
              case WindowHandles.SystemPower.BatteryFlag.Critical:
                base.Value = (object)"CRITICAL";
                break;
              case WindowHandles.SystemPower.BatteryFlag.Charging:
                base.Value = (object)"CHARGING";
                break;
            }
          }
          else
          {
            switch (flgBattery)
            {
              case WindowHandles.SystemPower.BatteryFlag.NoSystemBattery:
                base.Value = (object)"NO_SYSTEM_BATTERY";
                break;
              case WindowHandles.SystemPower.BatteryFlag.Unknown:
                base.Value = (object)"UNKNOWN";
                break;
              default:
                break;
            }
          }
          base.Unavailable();
        }
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}