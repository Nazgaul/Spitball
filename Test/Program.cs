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
                ConfigurationManager.AppSettings["AzureSearchKey"]);

            builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();

            

            var service = container.Resolve<IPlacesSearch>();
            //await service.SearchNearbyAsync("pizza",SearchRequestFilter.None,new GeoPoint()
            //{
            //    Latitude
            //})

            Console.ReadLine();
        }
    }
}
