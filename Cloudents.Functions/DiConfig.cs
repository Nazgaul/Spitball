using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AzureFunctions.Autofac.Configuration;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
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
                    Db = GetEnvironmentVariable("ConnectionString")
                };


                builder.Register(_ => keys).As<IConfigurationKeys>();
                builder.RegisterModule<ModuleDb>();
            });
        }

        private static string GetEnvironmentVariable(string name)
        {
            return name + ": " +
                   System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
