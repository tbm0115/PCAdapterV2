using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AdapterContext.DataModel;

namespace PCAdapter.GraphQL.Models
{
    public class PCAdapterRepository
    {
        private readonly AdapterContext.AdapterModel _ctx;

        public PCAdapterRepository(string connectionString)
        {
            _ctx = new AdapterContext.AdapterModel(connectionString);
        }

        public async Task<List<AdapterConfig>> GetAllAdapters()
        {
            return await _ctx.Adapters.Include("Items").ToListAsync();
        }
        public async Task<List<DataItem>> GetAllDataItems()
        {
            return await _ctx.DataItems.ToListAsync();
        }
        public async Task<List<Duration>> GetAllDurations()
        {
            return await _ctx.Durations.Include("Item").ToListAsync();
        }
    }
}
