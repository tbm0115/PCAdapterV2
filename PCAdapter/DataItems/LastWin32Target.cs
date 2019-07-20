using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AdapterContext;


namespace PCAdapter
{
  public class LastWin32Target : ADataItem
  {
    public new string Name => "Last Win32 Target";
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
        base.Value = (object)new Win32Exception(Marshal.GetLastWin32Error()).TargetSite;
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}