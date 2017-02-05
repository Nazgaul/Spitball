using Autofac;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Storage;


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
            Infrastructure.Mail.RegisterIoc.Register();

            Ioc.ContainerBuilder.RegisterModule<SearchModule>();

            Ioc.ContainerBuilder.RegisterModule<WriteServiceModule>();
            ReadServices.RegisterIoc.Register();
            Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            Ioc.ContainerBuilder.RegisterModule<CommandsModule>();
            Ioc.ContainerBuilder.RegisterModule<AiModule>();

            Ioc.ContainerBuilder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("ServiceBusConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("ServiceBusHubName"))
            .InstancePerLifetimeScope();

            //Unity = new UnityContainer();
            Ioc.ContainerBuilder.RegisterModule<WorkerRoleModule>();
            Ioc.Build();
            // Ioc.Resolve<IBlobProvider>();
        }

        public T Resolve<T>(string name)
        {
            return Ioc.Resolve<T>(name);
        }
    }
}
