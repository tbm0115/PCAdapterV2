using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class ActiveWindowWidth : ADataItem
  {
    public new string Name => "Active Window Width";
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
        WindowHandles.Rect lpRect;
        if (WindowHandles.GetWindowRect(WindowHandles.GetForegroundWindow(), out lpRect))
        {
          base.Value = (object)(lpRect.Right - lpRect.Left);
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