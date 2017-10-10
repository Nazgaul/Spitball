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
                ConfigurationManager.AppSettings["AzureSearchKey"], null);

            builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();



            var service = container.Resolve<IAI>();
            var result = await service.InterpretStringAsync(new Cloudents.Core.Request.AiQuery("document on war"));
            var result2 = await service.InterpretStringAsync(new Cloudents.Core.Request.AiQuery("document on war"));

            //await service.SearchNearbyAsync("pizza",SearchRequestFilter.None,new GeoPoint()
            //{
            //    Latitude
            //})

            Console.ReadLine();
        }
    }
}
