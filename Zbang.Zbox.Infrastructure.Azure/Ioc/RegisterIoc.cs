using Zbang.Zbox.Infrastructure.Azure.Blob;
using Zbang.Zbox.Infrastructure.Azure.MediaServices;
using Zbang.Zbox.Infrastructure.Azure.Queue;
using Zbang.Zbox.Infrastructure.Azure.Search;
using Zbang.Zbox.Infrastructure.Azure.Storage;
using Zbang.Zbox.Infrastructure.Azure.Table;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.MediaServices;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Zbox.Infrastructure.Azure.Ioc
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;
            ioc.RegisterType<IMediaSevicesProvider, MediaSevicesProvider>(LifeTimeManager.Singleton);

            ioc.RegisterType<IBlobProvider, BlobProvider>(LifeTimeManager.PerHttpRequest);
            
            ioc.RegisterType<ITableProvider, TableProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IQueueProvider, QueueProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<ILocalStorageProvider, LocalStorageProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IUniversityWriteSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IUniversityReadSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);

        }
    }
}
