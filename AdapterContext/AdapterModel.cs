using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterContext
{
  public class AdapterModel:DbContext
  {
    public DbSet<global::AdapterContext.DataModel.AdapterConfig> Adapters { get; set; }
    public DbSet<global::AdapterContext.DataModel.DataItem> DataItems { get; set; }
    public DbSet<global::AdapterContext.DataModel.Duration> Durations { get; set; }

    public AdapterModel():base("DefaultConnection")
    {

    }
    public AdapterModel(string cnnString):base(nameOrConnectionString: cnnString)
    {

    }


  }
}
