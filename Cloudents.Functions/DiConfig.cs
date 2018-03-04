using System;
using System.Reflection;
using Autofac;
using AzureFunctions.Autofac.Configuration;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;

namespace Cloudents.Functions
{
    class DiConfig
    {
        public DiConfig()
        {
            DependencyInjection.Initialize(builder =>
            {
                //Implicity registration
                var keys = new ConfigurationKeys
                {
                    Db = GetEnvironmentVariable("ConnectionString"),
                    Search = new SearchServiceCredentials(
                        GetEnvironmentVariable("SearchServiceName"),
                        GetEnvironmentVariable("SearchServiceAdminApiKey")),
                    MailGunDb = GetEnvironmentVariable("MailGunConnectionString")
                    //Redis = GetEnvironmentVariable("Redis"),
                    //Storage = GetEnvironmentVariable("AzureWebJobsStorage")
                };


                builder.Register(_ => keys).As<IConfigurationKeys>();

                builder.RegisterSystemModules(
                    Core.Enum.System.Function,
                    Assembly.Load("Cloudents.Infrastructure.Framework"),
                    //Assembly.Load("Cloudents.Infrastructure.Storage"),
                    Assembly.Load("Cloudents.Infrastructure"),
                    Assembly.Load("Cloudents.Core"));


                //builder.RegisterModule<ModuleCore>();
                //builder.RegisterModule<ModuleDb>();
                //builder.RegisterModule<ModuleReadDb>();
                //builder.RegisterModule<ModuleAzureSearch>();
                //builder.RegisterModule<ModuleMail>();
                builder.RegisterType<RestClient>().As<IRestClient>()
                    .SingleInstance();
                //builder.RegisterType<BinarySerializer>().As<IBinarySerializer>().SingleInstance();
            });
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
