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
using Cloudents.Core.Request;
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

          //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);
            var container = builder.Build();

            var services = container.Resolve<IUniversitySearch>();
            var location = new GeoPoint()
            {
                Latitude = 31.889692,
                Longitude = 34.812241
            };

            var result = await services.SearchAsync("East Carolina", location, default);
            Console.ReadLine();
        }
    }
}
