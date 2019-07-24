using AdapterContext.DataModel;
using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCAdapter.GraphQL.Models
{
    public class AdapterConfigType : ObjectGraphType<AdapterConfig>
    {
        public AdapterConfigType()
        {
            Field(o => o.Id);
            Field(o => o.Name);
            Field(o => o.PollRate);
            Field(o => o.LastStarted);
        }
    }
    public class AdapterQuery:ObjectGraphType
    {
        public AdapterQuery(AdapterContext.AdapterModel ctx)
        {

            Field<AdapterConfigType>(
                name: "adapterconfig",
                arguments: new QueryArguments(new QueryArgument<StringGraphType> { Name = "name" }),
                resolve: context =>
                {
                    var name = context.GetArgument<string>("name");
                    return ctx.Adapters.Where(o => o.Name.ToLower() == name.ToLower()).ToList();
                }
            );


        }
    }
}