using System;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Search.Job;

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
                SystemUrl = "https://dev.spitball.co",
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


            var mapper = container.Resolve<ICommandBus>();
            var commnad = new CreateCourseCommand("ram", 920);
            var p = await mapper.DispatchAsync<CreateCourseCommand, CreateCourseCommandResult>(commnad, default);
            var p2 = await mapper.DispatchAsync<CreateCourseCommand, CreateCourseCommandResult>(commnad, default);
            Console.ReadLine();
            // var model = SearchQuery.Document(new [] {"microsoft"}, null, null, null, 0, SearchRequestSort.None, null);
        }
    }
}
