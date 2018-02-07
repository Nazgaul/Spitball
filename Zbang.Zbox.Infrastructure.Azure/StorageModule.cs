using Autofac;
using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Azure.MediaServices;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Azure.Table;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure
{
   
    public class StorageModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediaServicesProvider>().As<IMediaServicesProvider>().SingleInstance();
            builder.RegisterType<BlobProvider>().As<IBlobProvider>().As<ICloudBlockProvider>().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<TableProvider>().As<ITableProvider>();
            builder.RegisterType<QueueProvider>().As<IQueueProvider>().As<IQueueProviderExtract>();
            builder.RegisterType<LocalStorageProvider>().As<ILocalStorageProvider>();
            builder.RegisterGeneric(typeof(BlobProvider2<>)).As(typeof(IBlobProvider2<>));
        }
    }
}
