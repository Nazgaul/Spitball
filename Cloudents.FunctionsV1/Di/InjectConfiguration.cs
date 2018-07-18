﻿using System;
using System.Reflection;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Host.Config;

namespace Cloudents.FunctionsV1.Di
{
    public class InjectConfiguration : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            var services = new ContainerBuilder();
            RegisterServices(services);
            var container = services.Build();
            //var serviceProvider = services.BuildServiceProvider(true);

            context
                .AddBindingRule<InjectAttribute>()
                .Bind(new InjectBindingProvider(container));

            var registry = context.Config.GetService<IExtensionRegistry>();
            var filter = new ScopeCleanupFilter();
            registry.RegisterExtension(typeof(IFunctionInvocationFilter), filter);
            registry.RegisterExtension(typeof(IFunctionExceptionFilter), filter);
        }
        private void RegisterServices(ContainerBuilder builder)
        {
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
                Core.Enum.System.FunctionV1,
                //Assembly.Load("Cloudents.Infrastructure.Framework"),
                //Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure.Data"),
                Assembly.Load("Cloudents.Core"));


           


        }

        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
