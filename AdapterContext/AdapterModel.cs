using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdapterContext
{
    public class AdapterModel : DbContext
    {
        public DbSet<global::AdapterContext.DataModel.AdapterConfig> Adapters { get; set; }
        public DbSet<global::AdapterContext.DataModel.DataItem> DataItems { get; set; }
        public DbSet<global::AdapterContext.DataModel.Duration> Durations { get; set; }

        public AdapterModel() : base("DefaultConnection")
        {

        }
        public AdapterModel(string cnnString) : base(nameOrConnectionString: cnnString)
        {

        }

        public virtual IQueryable<AdapterContext.DataModel.Duration> GetDuringDataItems(int[] ids)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            foreach (int id in ids)
            {
                table.Rows.Add(id);
            }
            var @idList = new SqlParameter("@idSource",  table);
            @idList.SqlDbType = System.Data.SqlDbType.Structured;
            @idList.TypeName = "mtc.IDList";

            var results = ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<AdapterContext.DataModel.Duration>("[mtc].[GetDuringDataItems] @DataItemIds=@idSource", @idList);

            var resultIds = results.Select(o => o.Id);
            return this.Durations.Include("Item").Where(o => resultIds.Contains(o.Id));
        }

    }
}
