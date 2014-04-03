using System;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mvc4WebRole.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var iocFactory = Zbox.Infrastructure.Ioc.IocFactory.Unity;
           // var container = new UnityContainer();
            RegisterTypes(iocFactory);
            return  iocFactory.unityContainer;// container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IocFactory container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            container.RegisterType<IUserProfile, UserProfile>();
            container.RegisterType<IEmailVerfication, EmailVerification>(LifeTimeManager.Singleton);


            //we need that for blob getting the blob container url
            container.Resolve<IBlobProvider>();
            container.Resolve<IThumbnailProvider>();

            container.Resolve<Zbang.Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
        }
    }
}
