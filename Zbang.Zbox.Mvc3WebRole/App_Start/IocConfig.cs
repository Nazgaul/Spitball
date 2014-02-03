using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure.ShortUrl;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;
using Zbang.Zbox.Mvc3WebRole.Factories;
using Zbang.Zbox.ReadServices;

namespace Zbang.Zbox.Mvc3WebRole.App_Start
{
    public class IocConfig
    {
        public static void RegisterIoc()
        {
            Zbang.Zbox.Infrastructure.RegisterIoc.Register();
            Zbang.Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbang.Zbox.Domain.Services.RegisterIoc.Register();
            
            Zbang.Zbox.ReadServices.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            //Zbang.Zbox.Domain.DataAccess.

           // Zbang.Zbox.Infrastructure.Ioc.IocFactory.Unity

            var iocFactory = Zbox.Infrastructure.Ioc.IocFactory.Unity; //new UnityControllerFactory();

            DependencyResolver.SetResolver(new Zbox.Infrastructure.Ioc.UnityDependencyResolver());
            iocFactory.Resolve<IBlobProvider>();
            iocFactory.Resolve<IThumbnailProvider>();
            //BoxesQueryFactory.Init(unityFactory.Resolve<IShortCodesCache>(), unityFactory.Resolve<IZboxReadService>());
            //TagsQueryFactory.Init(unityFactory.Resolve<IShortCodesCache>(), unityFactory.Resolve<IZboxReadService>());
            //DashBoardQueryFactory.Init(unityFactory.Resolve<IShortCodesCache>(), unityFactory.Resolve<IZboxReadService>());
        }
    }
}