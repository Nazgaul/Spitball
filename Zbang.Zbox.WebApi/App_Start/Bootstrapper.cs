using System.Web.Http;
using Microsoft.Practices.Unity;

namespace Zbang.Zbox.WebApi
{
    public static class Bootstrapper
    {
        public static void Initialise()
        {
            var container = BuildUnityContainer();
            
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }

        private static IUnityContainer BuildUnityContainer()
        {
//            var unityFactory = new UnityControllerFactory();
            //return unityFactory.GetContainer();
            return null;
           
        }
    }
}