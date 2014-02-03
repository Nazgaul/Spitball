using System.Web.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    public class IocConfig
    {
        public static void RegisterIoc()
        {
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();
            
            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            
            //Zbang.Zbox.Domain.DataAccess.

           // Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity

            var iocFactory = Zbox.Infrastructure.Ioc.IocFactory.Unity; //new UnityControllerFactory();
            iocFactory.RegisterType<IUserProfile, UserProfile>();
            iocFactory.RegisterType<IEmailVerfication, EmailVerification>(LifeTimeManager.Singleton);

            //singalR
            //Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver = new Zbang.Zbox.Infrastructure.Ioc.SignalRDependencyResolver();
            //Mvc
            DependencyResolver.SetResolver(new Zbox.Infrastructure.Ioc.UnityDependencyResolver());
            //WebApi
            //System.Web.Http.GlobalConfiguration.Configuration.DependencyResolver = new Zbox.Infrastructure.Ioc.UnityDependencyResolver();
            
            //we need that for blob getting the blob container url
            iocFactory.Resolve<IBlobProvider>();
            iocFactory.Resolve<IThumbnailProvider>();

            iocFactory.Resolve<Zbang.Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
        }

       
    }

    

}