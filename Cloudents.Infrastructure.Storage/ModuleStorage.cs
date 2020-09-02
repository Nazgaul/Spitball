using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Storage;

namespace Cloudents.Infrastructure.Storage
{
    public class ModuleStorage : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Storage;
                return new CloudStorageProvider(key.ConnectionString);
            }).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<BlobProviderContainer>().As<IBlobProvider>();


            builder.RegisterType<FilesBlobProvider>()
                .As<IDocumentDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.File);

            builder.RegisterType<BlobProviderContainer>()
                .As<IChatDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Chat)
                .WithParameter("container", StorageContainer.Chat);


            builder.RegisterType<UserDirectoryBlobProvider>()
                .As<IUserDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.User);

            builder.RegisterType<StudyRoomBlobProvider>().As<IStudyRoomBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.StudyRoom);

            builder.RegisterType<AdminDirectoryBlobProvider>()
                .As<IAdminDirectoryBlobProvider>()
                .Keyed<IBlobProvider>(StorageContainer.Admin);


            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();
            builder.RegisterType<ServiceBusProvider>().As<IServiceBusProvider>();
        }
    }
}