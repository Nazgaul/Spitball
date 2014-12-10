using System;
using Microsoft.Practices.Unity;
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
        #region Unity Container
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var iocFactory = IocFactory.Unity;
           // var container = new UnityContainer();
            RegisterTypes(iocFactory);
            return  iocFactory.UnityContainer;// container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="iocContainer">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        private static void RegisterTypes(IocFactory iocContainer)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // container.RegisterType<IProductRepository, ProductRepository>();
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            //iocContainer.RegisterType<IUserProfile, UserProfile>();


            //we need that for blob getting the blob container url
            iocContainer.Resolve<IBlobProvider>();
            iocContainer.Resolve<IAcademicBoxThumbnailProvider>();

            iocContainer.Resolve<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
        }
    }
}
