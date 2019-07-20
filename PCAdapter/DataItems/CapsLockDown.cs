using System;
using System.Threading.Tasks;
using AdapterContext;
using Microsoft.VisualBasic.Devices;

namespace PCAdapter
{
  public class CapsLockDown : ADataItem
  {
    public new string Name => "Caps Lock Down";
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
        base.Value = !new Keyboard().CapsLock ? (object)"INACTIVE" : (object)"ACTIVE";
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}