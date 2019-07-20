using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class EnvironmentUsername: ADataItem
  {
    public new string Name => "Environment Username";
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
        base.Value = (object)Environment.UserName;
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}