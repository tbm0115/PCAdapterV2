using System;
using System.Drawing;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class CursorY : ADataItem
  {
    public new string Name => "Cursor Y Position";
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
        Point lpPoint;
        if (WindowHandles.GetCursorPos(out lpPoint))
        {
          base.Value = (object)lpPoint.Y;
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