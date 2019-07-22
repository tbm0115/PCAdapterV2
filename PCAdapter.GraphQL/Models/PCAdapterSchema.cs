using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;

namespace PCAdapter.GraphQL.Models
{
    public class AdapterConfigType : ObjectGraphType<AdapterContext.DataModel.AdapterConfig>
    {
        public AdapterConfigType()
        {
            Field(o => o.Id);
            Field(o => o.Name);
            Field(o => o.LastStarted);
            Field(o => o.PollRate);
            Field(o => o.Items, type: typeof(ListGraphType<DataItemType>));
        }
    }
    public class DataItemType: ObjectGraphType<AdapterContext.DataModel.DataItem>
    {
        public DataItemType()
        {
            Field(o => o.Id);
            Field(o => o.Name);
            Field(o => o.IsActive);
            Field(o => o.TickFrequency);
        }
    }
    public class PCAdapterSchema:Schema
    {
        public PCAdapterSchema(IDependencyResolver resolver):base(resolver)
        {
            Query = resolver.Resolve<AdapterConfigType>();
        }
    }
}
