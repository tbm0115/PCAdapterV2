using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class ACStatus : ADataItem
  {
    public new string Name => "AC Status";
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
          switch (sps.LineStatus)
          {
            case WindowHandles.SystemPower.ACLineStatus.Offline:
              base.Value = (object)"OFFLINE";
              break;
            case WindowHandles.SystemPower.ACLineStatus.Online:
              base.Value = (object)"ONLINE";
              break;
            case WindowHandles.SystemPower.ACLineStatus.Unknown:
              base.Value = (object)"UNKNOWN";
              break;
            default:
              base.Unavailable();
              break;
          }
        }
        else
        {
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