﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Extension;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Sync;
using Cloudents.FunctionsV2.System;
using Cloudents.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Reflection;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Cloudents.FunctionsV2.Di
{
    public class AutofacServiceProviderBuilder : IServiceProviderBuilder
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public AutofacServiceProviderBuilder(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IServiceProvider Build()
        {
            // Get a setting from the configuration.
            Debug.WriteLine(_configuration["Setting"]);

            // ReSharper disable once CollectionNeverUpdated.Local - We need that because otherwise the inject fails
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();


            var keys = new ConfigurationKeys(
                _configuration["SiteEndPoint"] ?? "https://www.spitball.co")
            {
                Db = new DbConnectionString(_configuration["ConnectionString"], _configuration["Redis"]),
                Redis = _configuration["Redis"],
                Search = new SearchServiceCredentials(
                    _configuration["SearchServiceName"],
                        _configuration["SearchServiceAdminApiKey"],
                    bool.Parse(_configuration["IsDevelop"])
                ),
                ServiceBus = _configuration["AzureWebJobsServiceBus"],
                //MailGunDb = GetEnvironmentVariable("MailGunConnectionString"),
                //BlockChainNetwork = GetEnvironmentVariable("BlockChainNetwork"),
                Storage = _configuration["AzureWebJobsStorage"]
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();

            builder.RegisterSystemModules(
                Core.Enum.System.Function,
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));

            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance();



            builder.Register(c =>
            {
                var logger = _loggerFactory.CreateLogger("logger");
                return new Logger(logger);
            }).As<Core.Interfaces.ILogger>();

            builder.RegisterType<QuestionDbToSearchSync>()
                .Keyed<IDbToSearchSync>(SyncType.Question).SingleInstance();
            builder.RegisterType<UniversityDbToSearchSync>()
                .Keyed<IDbToSearchSync>(SyncType.University).SingleInstance();
            builder.RegisterType<DocumentDbToSearchSync>()
                .Keyed<IDbToSearchSync>(SyncType.Document).SingleInstance();




            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsClosedTypesOf(typeof(ISystemOperation<>));
           
            builder.Populate(services); // Populate is needed to have support for scopes.
            return new AutofacServiceProvider(builder.Build());
        }
    }
}