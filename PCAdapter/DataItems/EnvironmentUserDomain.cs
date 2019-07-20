using System;
using System.Threading.Tasks;
using AdapterContext;

namespace PCAdapter
{
  public class EnvironmentUserDomain : ADataItem
  {
    public new string Name => "Environment User Domain";
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
        base.Value = (object)Environment.UserDomainName;
      }
      catch (Exception ex)
      {
        base.Unavailable();
      }
    }
  }
}