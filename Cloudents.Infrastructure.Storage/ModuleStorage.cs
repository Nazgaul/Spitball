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
            //builder.RegisterType<UserDirectoryBlobProvider>().As<IUserDirectoryBlobProvider>();

            builder.RegisterType<FilesBlobProvider>()
                .As<IDocumentDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.File);

            //.WithParameter("container", StorageContainer.Document)
            //builder.RegisterType<BlobProviderContainer>()
            //    .As<IQuestionsDirectoryBlobProvider>()
            //    .Keyed<IBlobProvider>(StorageContainer.QuestionsAndAnswers)
            //    .WithParameter("container", StorageContainer.QuestionsAndAnswers);
            builder.RegisterType<BlobProviderContainer>()
                .As<IChatDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Chat)
                .WithParameter("container", StorageContainer.Chat);

            builder.RegisterType<UserDirectoryBlobProvider>()
                .As<IUserDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.User);

            //.WithParameter("container", StorageContainer.User);

            //builder.RegisterType<BlobProviderContainer>()
            //    .As<IRequestTutorDirectoryBlobProvider>()
            //    //.Keyed<IBlobProvider>(StorageContainer.RequestTutor)
            //    .WithParameter("container", StorageContainer.RequestTutor);

            //RequestTutor

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