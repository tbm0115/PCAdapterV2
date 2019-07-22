using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PCAdapter.WebAPI.Controllers
{
    [RoutePrefix("api/AdapterConfigs")]
    public class AdapterConfigsController : AdapterAPIController
    {
        // GET api/values
        [HttpGet]
        [Route("")]
        public IEnumerable<AdapterContext.DataModel.AdapterConfig> Get()
        {
            return this._ctx.Adapters.Include("Items").ToList();// new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet]
        [Route("{id:int}")]
        public AdapterContext.DataModel.AdapterConfig Get(int id)
        {
            var obj = this._ctx.Adapters.Include("Items").FirstOrDefault(o => o.Id == id);
            if (obj != null)
            {
                return obj;
            }
            else
            {
                return null;
            }
        }
        [HttpGet]
        [Route("{id:int}/DataItems")]
        public List<AdapterContext.DataModel.DataItem> GetDataItems(int id)
        {
            return this._ctx.Adapters.Include("Items").Where(o => o.Id == id).SelectMany(o => o.Items).Distinct().OrderBy(o => o.Name).ToList();
        }

        // POST api/values
        [HttpPost]
        [Route("")]
        public void Post([FromBody]AdapterContext.DataModel.AdapterConfig adapterConfig)
        {
            this._ctx.Adapters.Add(adapterConfig);
            this._ctx.Entry(adapterConfig).State = System.Data.Entity.EntityState.Added;
        }

        // PUT api/values/5
        [HttpPut]
        [Route("{id:int}")]
        public void Put(int id, [FromBody]AdapterContext.DataModel.AdapterConfig adapterConfig)
        {
            var obj = this._ctx.Adapters.Include("Item").FirstOrDefault(o => o.Id == adapterConfig.Id);
            if (obj != null)
            {
                obj.Items = adapterConfig.Items;
                obj.PollRate = adapterConfig.PollRate;
                this._ctx.Entry(obj).State = System.Data.Entity.EntityState.Modified;
                this._ctx.SaveChanges();
            }
            else
            {
                NotFound();
            }
        }

        // DELETE api/values/5
        [HttpDelete]
        [Route("{id:int}")]
        public void Delete(int id)
        {
            throw new NotSupportedException("Cannot delete Adapter Configurations. This is managed in the system.");
        }
    }
}
