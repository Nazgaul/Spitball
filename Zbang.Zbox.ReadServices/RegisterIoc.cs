using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ReadServices
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;

            ioc.RegisterType<IZboxCacheReadService, ZboxCacheReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IDocumentDbReadService, DocumentDbReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadService, ZboxReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IUniversityWithCode, ZboxReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadServiceWorkerRole, ZboxReadServiceWorkerRole>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadSecurityReadService, BaseReadService>(LifeTimeManager.PerHttpRequest);

        }
    }
}
