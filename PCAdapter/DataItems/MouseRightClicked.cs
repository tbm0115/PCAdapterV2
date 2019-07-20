using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class MouseRightClicked : ADataItem
  {
    public new string Name => "Mouse Right Clicked";
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
        base.Value = !WindowHandles.GetAsyncKeyState((ushort)2) ? (object)"INACTIVE" : (object)"ACTIVE";
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}