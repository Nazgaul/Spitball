using System.Linq;
using System.Web.Mvc;
using Microsoft.Practices.Unity.Mvc;
using Zbang.Cloudents.Mobile;
using Zbang.Cloudents.Mvc4WebRole;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(UnityWebActivator), "Start")]

namespace Zbang.Cloudents.Mobile
{
    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
// ReSharper disable once UnusedMember.Global
        public static void Start() 
        {
            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));

            // TODO: Uncomment if you want to use PerRequestLifetimeManager
            //Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(UnityPerRequestHttpModule));
        }
    }
}