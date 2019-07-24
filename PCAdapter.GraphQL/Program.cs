using GraphQL;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCAdapter.GraphQL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var schema = Schema.For(@"
                type AdapterConfig {
                    id: Int
                    name: String
                    pollRate: Int
                }

                type AdapterQuery {
                    getAdapterConfig(name: String): AdapterConfig
                }
            ", o =>
            {
                o.Types.Include<PCAdapter.GraphQL.Models.AdapterQuery>();
            });

            var json = schema.Execute(o => o.Query = $"{{ config(name: \"Default\") {{ id name }} }}");

            Console.WriteLine(json);

            Console.ReadLine();
        }
    }
}