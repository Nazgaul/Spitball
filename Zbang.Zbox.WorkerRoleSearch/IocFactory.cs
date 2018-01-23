using System.Configuration;
using Autofac;
using Cloudents.Infrastructure;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
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
            builder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();

            //builder.RegisterType<JaredSendPush>()
            //    .As<IJaredPushNotification>()
            //    .WithParameter("connectionString", "Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=")
            //    .WithParameter("hubName", "jared-spitball")
            //    .InstancePerLifetimeScope();


            var infrastructureModule = new WriteModuleBase(
                new SearchServiceCredentials(ConfigFetcher.Fetch("AzureSeachServiceName"),
                    ConfigFetcher.Fetch("AzureSearchKey")));

            //  builder.RegisterType<GoogleSheet>().As<IGoogleSheet>();
            builder.RegisterModule(infrastructureModule);

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
