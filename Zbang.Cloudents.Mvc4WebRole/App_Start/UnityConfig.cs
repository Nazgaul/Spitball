using System;
using System.Web.Mvc;
using Autofac.Integration.Mvc;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mvc4WebRole
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {

        //#region Unity Container
        //private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        //{
        //    var iocFactory = IocFactory.Unity;
        //   // var container = new UnityContainer();
        //    RegisterTypes(iocFactory);
        //    return  iocFactory.UnityContainer;// container;
        //});

        ///// <summary>
        ///// Gets the configured Unity container.
        ///// </summary>
        //public static IUnityContainer GetConfiguredContainer()
        //{
        //    return Container.Value;
        //}
        //#endregion

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
            DependencyResolver.Current.GetService<IAcademicBoxThumbnailProvider>();

            DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
        }
    }
}
