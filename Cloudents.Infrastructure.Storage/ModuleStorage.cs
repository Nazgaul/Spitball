using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Storage
{
    [UsedImplicitly]
    public class ModuleStorage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Storage;
                return new CloudStorageProvider(key);
            }).SingleInstance().AsImplementedInterfaces();

            //builder.RegisterType<BlobProvider>().AsImplementedInterfaces();
            builder.RegisterType<BlobProviderContainer>().As<IBlobProvider>();

            builder.RegisterType<BlobProviderContainer>()
                .As<IDocumentDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.File)
                .WithParameter("container", StorageContainer.File);

            //.WithParameter("container", StorageContainer.Document)
            builder.RegisterType<BlobProviderContainer>()
                .As<IQuestionsDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.QuestionsAndAnswers)
                .WithParameter("container", StorageContainer.QuestionsAndAnswers);
            builder.RegisterType<BlobProviderContainer>()
                .As<IChatDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Chat)
                .WithParameter("container", StorageContainer.Chat);

            builder.RegisterType<BlobProviderContainer>()
                .As<IUserDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Chat)
                .WithParameter("container", StorageContainer.User);

            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();
            builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>();
        }
    }

    //[ModuleRegistration(Application.Enum.System.WorkerRole)]
    //public class ModuleTempStorage : Module
    //{
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        builder.Register(c =>
    //        {
    //            var key = c.Resolve<IConfigurationKeys>().LocalStorageData;
    //            return new TempStorageProvider(c.Resolve<ILogger>(), key);
    //        }).As<ITempStorageProvider>();
    //    }
    //}
}