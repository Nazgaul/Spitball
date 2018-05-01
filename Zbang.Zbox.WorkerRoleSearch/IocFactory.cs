using System.Reflection;
using Autofac;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.ReadServices;
using InfrastructureModule = Zbang.Zbox.Infrastructure.InfrastructureModule;

namespace Zbang.Zbox.WorkerRoleSearch
{
    internal class IocFactory
    {
        public const string UpdateSearchItem = "UpdateSearchItem";
        public const string UpdateSearchBox = "UpdateSearchBox";
        public const string UpdateSearchQuiz = "UpdateSearchQuiz";
        public const string UpdateSearchFlashcard = "UpdateSearchFlashcard";
        public const string UpdateSearchUniversity = "UpdateSearchUniversity";

        private IContainer Container { get; }

        public IocFactory()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<FileModule>();
            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<MailModule>();

            builder.RegisterModule<SearchModule>();

            builder.RegisterModule<WriteServiceModule>();
            builder.RegisterModule<ReadServiceModule>();
            builder.RegisterModule<CommandsModule>();
            var azureLocalResource = RoleEnvironment.GetLocalResource("ItemPreviewStorage");


            var keys = new ConfigurationKeys
            {
                Search = new SearchServiceCredentials(ConfigFetcher.Fetch("AzureSeachServiceName"),
                    ConfigFetcher.Fetch("AzureSearchKey")),
                Redis = "zboxcache.redis.cache.windows.net,abortConnect=false,allowAdmin=true,ssl=true,password=CxHKyXDx40vIS5EEYT0UfnVIR1OJQSPrNnXFFdi3UGI=",
                LocalStorageData = new LocalStorageData(azureLocalResource.RootPath, azureLocalResource.MaximumSizeInMegabytes),
                Storage = ConfigFetcher.Fetch("StorageConnectionString")
            };

            builder.Register(_ => keys).As<IConfigurationKeys>();

            builder.RegisterSystemModules(
                Cloudents.Core.Enum.System.WorkerRole,
                Assembly.Load("Cloudents.Infrastructure.Framework"),
                Assembly.Load("Cloudents.Infrastructure.Storage"),
                Assembly.Load("Cloudents.Infrastructure"),
                Assembly.Load("Cloudents.Core"));

            builder.RegisterModule<WorkerRoleModule>();
            Container = builder.Build();
        }

        public T Resolve<T>(string name)
        {
            return Container.ResolveNamed<T>(name);
        }

        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }
    }
}
