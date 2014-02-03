using System;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Routing;
using Zbang.Zbox.Infrastructure.ServiceModel.RestIoc;


namespace Zbang.Zbox.WcfRestService
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes();

           
            
        }

        private void RegisterRoutes()
        {
            // Edit the base address of Service1 by replacing the "Service1" string below
            var unityFactory = new UnityServiceHostFactory();
           // RouteTable.Routes.Add(new ServiceRoute("Auth", unityFactory, typeof(Zbox)));
            RouteTable.Routes.Add(new ServiceRoute("", unityFactory, typeof(Zbox)));
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
        }
    }
}
