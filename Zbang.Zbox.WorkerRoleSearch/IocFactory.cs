using Autofac;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;


namespace Zbang.Zbox.WorkerRoleSearch
{
    internal class IocFactory
    {
        public const string UpdateSearchItem = "UpdateSearchItem";
        public const string UpdateSearchBox = "UpdateSearchBox";
        public const string UpdateSearchQuiz = "UpdateSearchQuiz";
        public const string UpdateSearchFlashcard = "UpdateSearchFlashcard";
        public const string UpdateSearchUniversity = "UpdateSearchUniversity";

        public Infrastructure.Ioc.IocFactory Ioc { get; }
        public IocFactory()
        {
            Ioc = Infrastructure.Ioc.IocFactory.IocWrapper;

            Ioc.ContainerBuilder.RegisterModule<InfrastructureModule>();
            Ioc.ContainerBuilder.RegisterModule<DataModule>();
            Infrastructure.File.RegisterIoc.Register();
            Ioc.ContainerBuilder.RegisterModule<StorageModule>();
            Ioc.ContainerBuilder.RegisterModule<MailModule>();

            Ioc.ContainerBuilder.RegisterModule<SearchModule>();

            Ioc.ContainerBuilder.RegisterModule<WriteServiceModule>();
            Ioc.ContainerBuilder.RegisterModule<ReadServiceModule>();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            Ioc.ContainerBuilder.RegisterModule<CommandsModule>();
            Ioc.ContainerBuilder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();


            Ioc.ContainerBuilder.RegisterType<JaredSendPush>()
                .As<IJaredPushNotification>()
                .WithParameter("connectionString", "Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=")
                .WithParameter("hubName", "jared-spitball")
                .InstancePerLifetimeScope();

            Ioc.ContainerBuilder.RegisterModule<WorkerRoleModule>();
            Ioc.Build();

        }

        public T Resolve<T>(string name)
        {
            return Ioc.Resolve<T>(name);
        }
    }
}
