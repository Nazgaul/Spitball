using Autofac;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Query;

namespace Zbang.Zbox.ReadServices
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;

    //        ioc.RegisterType<IZboxCacheReadService, ZboxCacheReadService>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IDocumentDbReadService, DocumentDbReadService>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IZboxReadService, ZboxReadService>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IUniversityWithCode, ZboxReadService>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IZboxReadServiceWorkerRole, ZboxReadServiceWorkerRole>(LifeTimeManager.PerHttpRequest);
    //        ioc.RegisterType<IZboxReadSecurityReadService, BaseReadService>(LifeTimeManager.PerHttpRequest);

    //    }
    //}

    public class ReadServiceModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<ZboxCacheReadService>().As<IZboxCacheReadService>();
            builder.RegisterType<DocumentDbReadService>().As<IDocumentDbReadService>();
            builder.RegisterType<ZboxReadService>().As<IZboxReadService>().As< IUniversityWithCode>();
            builder.RegisterType<ZboxReadServiceWorkerRole>().As<IZboxReadServiceWorkerRole>();
            builder.RegisterType<BaseReadService>().As<IZboxReadSecurityReadService>();
            
        }
    }
}
