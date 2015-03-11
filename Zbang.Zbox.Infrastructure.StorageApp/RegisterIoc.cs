
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.StorageApp
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
            ioc.RegisterType<IMediaSevicesProvider, MediaSevicesProvider>(LifeTimeManager.Singleton);

            ioc.RegisterType<IBlobProvider, BlobProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IBlobUpload, BlobProvider>(LifeTimeManager.PerHttpRequest);
            //ioc.RegisterType<IBlobProductProvider, BlobProvider>();
            //ioc.RegisterType<ICloudBlockProvider, BlobProvider>();

            //ioc.RegisterType<ITableProvider, TableProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IQueueProvider, QueueProvider>(LifeTimeManager.PerHttpRequest);
            //ioc.RegisterType<IQueueProviderExtract, QueueProvider>(LifeTimeManager.PerHttpRequest);
            //ioc.RegisterType<ILocalStorageProvider, LocalStorageProvider>();
            //ioc.RegisterType<IdGenerator.IIdGenerator, Blob.IdGenerator>(LifeTimeManager.Singleton);
        }
    }
}
