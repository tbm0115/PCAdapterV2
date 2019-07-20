using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class BatteryLevel : ADataItem
  {
    public new string Name => "Battery Level";
    public new bool IsActive { get; set; }
    public new int TickFrequency { get; set; }
    private bool isTicking { get; set; }
    private WindowHandles.SystemPower _sps;
    public BatteryLevel():base()
    {
      this._sps = new WindowHandles.SystemPower();
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
      try
      {
        base.Value = (object)this._sps.BatteryCharge();
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}