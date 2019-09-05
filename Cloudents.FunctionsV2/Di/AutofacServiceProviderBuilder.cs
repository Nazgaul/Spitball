﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.FunctionsV2.Sync;
using Cloudents.FunctionsV2.System;
using Cloudents.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Diagnostics;
using System.Reflection;
using Cloudents.FunctionsV2.Services;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using ILogger = Cloudents.Core.Interfaces.ILogger;

namespace Cloudents.FunctionsV2.Di
{
    public class AutofacServiceProviderBuilder : IServiceProviderBuilder
    {
        private readonly IConfiguration _configuration;

        public AutofacServiceProviderBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IServiceProvider Build()
        {
            // Get a setting from the configuration.
            Debug.WriteLine(_configuration["Setting"]);

            // ReSharper disable once CollectionNeverUpdated.Local - We need that because otherwise the inject fails
            var services = new ServiceCollection();

            var builder = new ContainerBuilder();

            services.AddDataProtection(o =>
            {
                o.ApplicationDiscriminator = "spitball";
            }).PersistKeysToAzureBlobStorage(CloudStorageAccount.Parse(_configuration["AzureWebJobsStorage"]), "/spitball/keys/keys.xml");



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
                Storage = _configuration["AzureWebJobsStorage"]
            };



            //var assemblies = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            //    .Where(path =>
            //        Path.GetExtension(path) == ".dll" &&
            //        Path.GetFileNameWithoutExtension(path) 
            //            .Contains("Cloudents",
            //                StringComparison.OrdinalIgnoreCase))
            //    .Select(Assembly.Load).ToArray();


            builder.Register(_ => keys).As<IConfigurationKeys>();
            builder.RegisterAssemblyModules(
                Assembly.Load("Cloudents.Infrastructure.Storage"), //We need this because of event handler in question populate
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Persistence"));

            builder.RegisterType<HostUriService>().AsSelf().AsImplementedInterfaces();
            builder.RegisterType<DataProtectionService>().As<IDataProtectionService>();

            builder.RegisterType<RestClient>().As<IRestClient>()
                .SingleInstance();

            builder.RegisterType<Logger>().As<ILogger>();


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