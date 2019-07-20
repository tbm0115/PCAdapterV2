using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PCAdapter.WebAPI.Controllers
{
  [RoutePrefix("api/DataItems")]
  public class DataItemsController : AdapterAPIController
  {
    
    // GET api/values
    [HttpGet]
    [Route("")]
    public IEnumerable<AdapterContext.DataModel.DataItem> Get()
    {
      return this._ctx.DataItems.ToList();// new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet]
    [Route("{id:int}")]
    public  AdapterContext.DataModel.DataItem Get(int id)
    {
      var obj =  this._ctx.DataItems.FirstOrDefault(o => o.Id == id);
      if (obj != null)
      {
        return obj;
      }
      else
      {
        return null;
      }
    }

    // POST api/values
    [HttpPost]
    [Route("")]
    public void Post([FromBody]AdapterContext.DataModel.DataItem dataItem)
    {
      throw new NotSupportedException("Cannot create new DataItems. This is managed in the system.");
    }

    // PUT api/values/5
    [HttpPut]
    [Route("{id:int}")]
    public void Put(int id, [FromBody]AdapterContext.DataModel.DataItem dataItem)
    {
      var obj = this._ctx.DataItems.FirstOrDefault(o => o.Id == dataItem.Id);
      if (obj != null)
      {
        obj.IsActive = dataItem.IsActive;
        obj.TickFrequency = dataItem.TickFrequency;
        this._ctx.Entry(obj).State = EntityState.Modified;
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
      throw new NotSupportedException("Cannot delete DataItems. This is managed in the system.");
    }

  }
}
