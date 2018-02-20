using System;
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
                    Redis = GetEnvironmentVariable("Redis"),
                    Storage = GetEnvironmentVariable("AzureWebJobsStorage")
                };
                builder.Register(_ => keys).As<IConfigurationKeys>();
                builder.RegisterModule<ModuleCore>();
                builder.RegisterModule<ModuleDb>();
                builder.RegisterModule<ModuleReadDb>();
                builder.RegisterModule<ModuleAzureSearch>();
            });
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
