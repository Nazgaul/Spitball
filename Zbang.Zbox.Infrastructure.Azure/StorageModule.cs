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
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;
    //        ioc.RegisterType<IMediaServicesProvider, MediaSevicesProvider>(LifeTimeManager.Singleton);

    //        ioc.RegisterType<IBlobProvider, BlobProvider>(LifeTimeManager.PerHttpRequest);
    //        //ioc.RegisterType<IBlobUpload, BlobProvider>(LifeTimeManager.PerHttpRequest);
    //        //ioc.RegisterType<IBlobProductProvider, BlobProvider>();
    //        ioc.RegisterType<ICloudBlockProvider, BlobProvider>(LifeTimeManager.Singleton);

    //        ioc.RegisterType<ITableProvider, TableProvider>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IQueueProvider, QueueProvider>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IQueueProviderExtract, QueueProvider>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<ILocalStorageProvider, LocalStorageProvider>();
    //        ioc.RegisterType<IdGenerator.IIdGenerator, Blob.IdGenerator>(LifeTimeManager.Singleton);


    //        ioc.RegisterGeneric(typeof(IBlobProvider2<>), typeof(BlobProvider2<>));
    //    }
    //}
    public class StorageModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MediaServicesProvider>().As<IMediaServicesProvider>().SingleInstance();
            builder.RegisterType<BlobProvider>().As<IBlobProvider>().As<ICloudBlockProvider>().InstancePerLifetimeScope().AutoActivate();
            builder.RegisterType<TableProvider>().As<ITableProvider>();
            builder.RegisterType<QueueProvider>().As<IQueueProvider>().As<IQueueProviderExtract>();
            builder.RegisterType<LocalStorageProvider>().As<ILocalStorageProvider>();
            builder.RegisterType<Blob.IdGenerator>().As<IdGenerator.IIdGenerator>().SingleInstance();
            builder.RegisterGeneric(typeof(BlobProvider2<>)).As(typeof(IBlobProvider2<>));
        }
    }
}
