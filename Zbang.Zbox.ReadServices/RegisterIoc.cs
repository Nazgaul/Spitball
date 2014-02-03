using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.ReadServices
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var Ioc = IocFactory.Unity;

            Ioc.RegisterType<IZboxCacheReadService, ZboxCacheReadService>(LifeTimeManager.PerHttpRequest);
            Ioc.RegisterType<IZboxReadService,  ZboxReadService>(LifeTimeManager.PerHttpRequest);
            //Ioc.RegisterType<IZboxApiReadService, ZboxApiReadService>(LifeTimeManager.PerHttpRequest);
            Ioc.RegisterType<IZboxReadServiceWorkerRole, ZboxReadServiceWorkerRole>(LifeTimeManager.PerHttpRequest);

        }
    }
}
