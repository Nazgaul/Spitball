using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;

namespace ConsoleApp
{
    static class Program
    {
        static async Task Main()
        {
            var builder = new ContainerBuilder();
            //var infrastructureModule = new InfrastructureModule(
            //    ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
            //    ConfigurationManager.AppSettings["AzureSearchServiceName"],
            //    ConfigurationManager.AppSettings["AzureSearchKey"],
            //    ConfigurationManager.AppSettings["Redis"],
            //    ConfigurationManager.AppSettings["StorageConnectionString"]);

            //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();

            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,

                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"]
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterModule<ModuleRead>();

            
            // new LocalStorageData(Path.Combine(Directory.GetCurrentDirectory(), "Temp"), 500)));
            builder.RegisterModule<ModuleFile>();
            builder.RegisterModule<ModuleDb>();
            var container = builder.Build();
            var point = new GeoPoint()
            {
                Latitude = 40.7127753,
                Longitude = -74.0059728
            };

            var commandBus = container.Resolve<ICommandBus>();
            var command = new CreateUrlStatsCommand("ram", DateTime.UtcNow, "ram", "ram",
                0);

            await commandBus.DispatchAsync(command, default).ConfigureAwait(false);

            Console.ReadLine();
            // var model = SearchQuery.Document(new [] {"microsoft"}, null, null, null, 0, SearchRequestSort.None, null);
        }
    }
}
