using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.ReadServices
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.Unity;

            ioc.RegisterType<IZboxCacheReadService, ZboxCacheReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadService,  ZboxReadService>(LifeTimeManager.PerHttpRequest);
            //Ioc.RegisterType<IZboxApiReadService, ZboxApiReadService>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadServiceWorkerRole, ZboxReadServiceWorkerRole>(LifeTimeManager.PerHttpRequest);
            ioc.RegisterType<IZboxReadSecurityReadService, BaseReadService>(LifeTimeManager.PerHttpRequest);

        }
    }
}
