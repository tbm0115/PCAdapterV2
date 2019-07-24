using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace PCAdapter.GraphQL.Controllers
{
    [Route(Startup.GraphQlPath)]
    public class GraphQlController : ApiController
    {
        private AdapterContext.AdapterModel ctx;
        public GraphQlController()
        {
            this.ctx = new AdapterContext.AdapterModel();
        }

        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] GraphQlQuery query)
        {
            var schema = new Schema { Query = new PCAdapter.GraphQL.Models.AdapterQuery(ctx) };
            var result = await new DocumentExecuter().ExecuteAsync(x =>
            {
                x.Schema = schema;
                x.Query = query.Query;
                x.Inputs = query.Variables;
            });
            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }
            return Ok(result);
        }
    }
    public class GraphQlQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public Inputs Variables { get; set; }
    }
}