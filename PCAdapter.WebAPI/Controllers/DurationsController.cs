using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;

namespace PCAdapter.WebAPI.Controllers
{
    [RoutePrefix("api/Durations")]
    public class DurationsController : AdapterAPIController
    {
        private Expression<Func<AdapterContext.DataModel.Duration, DurationDTO>> selectExpression { get; set; }

        public DurationsController()
        {
            this.selectExpression = o => new DurationDTO
            {
                Id = o.Id,
                Started = o.Started,
                Ended = o.Ended,
                Timespan = o.Timespan,
                Value = o.Value
            };
        }
        public class DurationDTO
        {
            public int Id;
            public DateTime Started;
            public DateTime? Ended;
            public long? Timespan;
            public string Value;
        }

        // GET api/values
        [HttpGet]
        [Route("")]
        public IEnumerable<AdapterContext.DataModel.Duration> Get()
        {
            return this._ctx.Durations.Include("Item").OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();// new string[] { "value1", "value2" };
        }
        [HttpGet]
        [Route("start/min/{time:datetime}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetMinTime(DateTime time)
        {
            return this._ctx.Durations.Include("Item").Where(o => o.Started >= time).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("start/max/{time:datetime}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetMaxTime(DateTime time)
        {
            return this._ctx.Durations.Include("Item").Where(o => o.Started <= time).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("start/between/{min:datetime}/and/{max:datetime}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetBetweenTime(DateTime min, DateTime max)
        {
            return this._ctx.Durations.Include("Item").Where(o => o.Started >= min && o.Started <= max).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("dataitem/{id:int}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetByDataItem(int id)
        {
            return this._ctx.Durations.Include("Item").Where(o => o.Item.Id == id).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("dataitem/value/{value}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetByDataItemValue(string value)
        {
            return this._ctx.Durations.Include("Item").Where(o => o.Value.ToLower() == value.ToLower()).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("dataitem/{name}/from/{start:datetime}/to/{end:datetime}")]
        public IEnumerable<DurationDTO> GetByDataItemFromTo(string name, DateTime start, DateTime end)
        {
            var startTime = start.Date;
            var endTime = end.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            return this._ctx.Durations.Where(o =>
                 ((o.Started >= startTime && o.Started < endTime) || (o.Ended >= startTime && o.Ended <= endTime))
                 && o.Item.Name.ToLower() == name).Select(this.selectExpression).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("today")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetToday()
        {
            DateTime startOfToday = new DateTime(DateTime.UtcNow.Date.Ticks);
            DateTime endOfToday = startOfToday.AddHours(23).AddMinutes(59).AddSeconds(59);
            return this._ctx.Durations.Include("Item").Where(o => (o.Started >= startOfToday || (o.Ended >= startOfToday && o.Ended <= endOfToday))).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("today/{id:int}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetTodayByDataItem(int id)
        {
            DateTime startOfToday = new DateTime(DateTime.UtcNow.Date.Ticks);
            DateTime endOfToday = startOfToday.AddHours(23).AddMinutes(59).AddSeconds(59);
            return this._ctx.Durations.Include("Item").Where(o => (o.Started >= startOfToday || (o.Ended >= startOfToday && o.Ended <= endOfToday)) && o.Item.Id == id).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }
        [HttpGet]
        [Route("today/{name}")]
        public IEnumerable<AdapterContext.DataModel.Duration> GetTodayByDataItem(string name)
        {
            DateTime startOfToday = new DateTime(DateTime.UtcNow.Date.Ticks);
            DateTime endOfToday = startOfToday.AddHours(23).AddMinutes(59).AddSeconds(59);
            return this._ctx.Durations.Include("Item").Where(o => (o.Started >= startOfToday || (o.Ended >= startOfToday && o.Ended <= endOfToday)) && o.Item.Name.ToLower() == name.ToLower()).OrderByDescending(o => o.Started).ThenByDescending(o => o.Timespan).ToList();
        }


        // GET api/values/5
        [HttpGet]
        [Route("{id:int}")]
        public AdapterContext.DataModel.Duration Get(int id)
        {
            var obj = this._ctx.Durations.Include("Item").FirstOrDefault(o => o.Id == id);
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
        public void Post([FromBody]AdapterContext.DataModel.Duration duration)
        {
            this._ctx.Durations.Add(duration);
            this._ctx.Entry(duration).State = System.Data.Entity.EntityState.Added;
        }

        // PUT api/values/5
        [HttpPut]
        [Route("{id:int}")]
        public void Put(int id, [FromBody]AdapterContext.DataModel.Duration duration)
        {
            var obj = this._ctx.Durations.Include("Item").FirstOrDefault(o => o.Id == duration.Id);
            if (obj != null)
            {
                obj.Ended = duration.Ended;
                obj.Timespan = ((DateTime)obj.Ended - obj.Started).Ticks;
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
            throw new NotSupportedException("Cannot delete Durations. This is managed in the system.");
        }
    }
}
