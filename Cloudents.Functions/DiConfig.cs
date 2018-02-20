using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AzureFunctions.Autofac.Configuration;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Infrastructure.Search;

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
                builder.RegisterModule<ModuleDb>();
               // builder.RegisterModule<ModuleWrite>();
            });
        }

        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
