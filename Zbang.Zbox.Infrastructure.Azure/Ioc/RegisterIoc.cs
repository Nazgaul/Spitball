using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Azure.Search;
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

            ioc.RegisterType<IBlobProvider, Storage.BlobProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<ITableProvider, Storage.TableProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IQueueProvider, Storage.QueueProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<ILocalStorageProvider, Storage.LocalStorageProvider>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IUniversityWriteSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IUniversityReadSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);

        }
    }
}
