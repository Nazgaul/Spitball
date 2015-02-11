using System;
//using Microsoft.Practices.Unity;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mobile
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {

        public static void RegisterTypes()
        {

            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            IocFactory.Unity.ContainerBuilder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            IocFactory.Unity.ContainerBuilder.RegisterFilterProvider();

            var container = IocFactory.Unity.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //iocContainer.RegisterType<IUserProfile, UserProfile>();


            //we need that for blob getting the blob container url
            DependencyResolver.Current.GetService<IBlobProvider>();

            DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
        }
    }
}
