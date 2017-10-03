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


            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();


            var service = container.Resolve<IBookSearch>();

            var point = new GeoPoint()
            {
                Latitude = 34.8013,
                Longitude = 31.9321
            };
            var result = await service.BuyAsync("9780446556224", 250, default);

            var t = result;
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}
