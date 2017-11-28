using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core.Enum;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure;
using Environment = Cloudents.Infrastructure.Environment;

namespace Test
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            var infrastructureModule = new InfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"],
                ConfigurationManager.AppSettings["Redis"], Environment.Console);

            builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();


            var services = container.Resolve<IEngineProcess>();
            var result = await services.ProcessRequestAsync("study guides on war");


            //foreach (var service in services)
            //{
            //    service.SearchAsync("math",SearchRequestFilter.None,SearchRequestSort.None,null,0,default);
            //}




            //await service.SearchNearbyAsync("pizza",SearchRequestFilter.None,new GeoPoint()
            //{
            //    Latitude
            //})

            Console.ReadLine();
        }
    }
}
