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

            builder.RegisterType<BlobProviderContainer>()
                .As<IChatDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Chat)
                .WithParameter("container", StorageContainer.Chat);


            builder.RegisterType<StudyRoomSessionBlobProvider>()
                .As<IStudyRoomSessionBlobProvider>()
                .WithParameter("container", StorageContainer.StudyRoom);

            builder.RegisterType<UserDirectoryBlobProvider>()
                .As<IUserDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.User);


            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();
            builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>();
        }
    }
}