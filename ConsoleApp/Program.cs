using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Command;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Framework.Database;

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
                MailGunDb = ConfigurationManager.ConnectionStrings["MailGun"].ConnectionString,
                Search = new SearchServiceCredentials(

                    ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["StorageConnectionString"]
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterModule<ModuleRead>();
            builder.RegisterType<BinarySerializer>().As<IBinarySerializer>().SingleInstance();

            // new LocalStorageData(Path.Combine(Directory.GetCurrentDirectory(), "Temp"), 500)));
            builder.RegisterModule<ModuleFile>();
            builder.RegisterModule<ModuleDb>();
            builder.RegisterModule<ModuleCore>();
            builder.RegisterModule<ModuleMail>();
            builder.RegisterModule<ModuleAzureSearch>();

            var container = builder.Build();
            //z.Invoke(Database.System);
            var bus = container.Resolve<ICommandBus>();
            var command = new UpdateMailGunCommand(1533064);
            await bus.DispatchAsync(command, default).ConfigureAwait(false);
            var command2 = new CreateCourseCommand("ram", 920);
            var p = await bus.DispatchAsync<CreateCourseCommand, CreateCourseCommandResult>(command2, default).ConfigureAwait(false);
            await bus.DispatchAsync(command, default).ConfigureAwait(false);

            Console.WriteLine("Finish");
            Console.ReadLine();
        }
    }
}
