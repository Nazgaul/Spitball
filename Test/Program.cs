using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure;

namespace Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var infrastructureModule = new InfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"],
                ConfigurationManager.AppSettings["Redis"]);

            builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();



            var service = container.Resolve<IDocumentSearch>();
            var service2 = container.Resolve<IAI>();


            //var result3 = await service2.InterpretStringAsync("document on war");
            //var result4 = await service2.InterpretStringAsync("document on war");

            var result = await service.SearchAsync(
                new Cloudents.Core.Request.SearchQuery(new[] {"war"}, null, null, null, 0, SearchRequestSort.None), default);
            var result2 = await service.SearchAsync(
                new Cloudents.Core.Request.SearchQuery(new[] { "war" }, null, null, null, 0, SearchRequestSort.None), default);

            

            //await service.SearchNearbyAsync("pizza",SearchRequestFilter.None,new GeoPoint()
            //{
            //    Latitude
            //})

            Console.ReadLine();
        }
    }
}
