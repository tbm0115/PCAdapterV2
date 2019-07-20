using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class ActiveWindowEXE : ADataItem
  {
    public new string Name => "Active Window EXE Path";
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
        uint ProcessId;
        WindowHandles.GetWindowThreadProcessId(WindowHandles.GetForegroundWindow(), out ProcessId);
        using (Process processById = Process.GetProcessById((int)ProcessId))
          base.Value = (object)processById.MainModule.FileName;
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}