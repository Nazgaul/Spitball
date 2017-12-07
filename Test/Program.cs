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

            var service = container.Resolve<IDocumentCseSearch>();

            service.SearchAsync(new SearchQuery(
                new List<string> { "vector operations in 2-dimensions" }, 
                ), default);

            //var services = container.Resolve<IUniversitySearch>();
            //var location = new GeoPoint()
            //{
            //    Latitude = 40.695217,
            //    Longitude = -73.964510
            //};
           
            //var result = await services.SearchAsync("iowa",)
            Console.ReadLine();
        }
    }
}
