using System;
using System.Threading.Tasks;
using AdapterContext;


namespace PCAdapter
{
  public class ActiveWindowTitle : ADataItem
  {
    public new string Name => "Active Window Title";
    
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
        string activeWindowTitle = WindowHandles.GetActiveWindowTitle();
        if (!string.IsNullOrEmpty(activeWindowTitle))
          base.Value = (object)activeWindowTitle;
        else
          base.Unavailable();
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}