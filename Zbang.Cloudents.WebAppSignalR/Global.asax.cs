using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Zbang.Cloudents.WebAppSignalR
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            //Zbox.Infrastructure.RegisterIoc.Register();
          //  Microsoft.AspNet.SignalR.GlobalHost.DependencyResolver = new Zbang.Zbox.Infrastructure.Ioc.SignalRDependencyResolver();


            //string connectionString = "Endpoint=sb://zbangcloudents.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=pwEE7wUSxPY/AkJu4LFXG3eVjZHvv+sKWI16ajW51qE=";
            //if (Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
            //{
            //    GlobalHost.DependencyResolver.UseServiceBus(connectionString, "Cloudents");
            //}


            //var config = new Microsoft.AspNet.SignalR.HubConfiguration();
            //config.EnableJavaScriptProxies = false;

            //if (!Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
            //{
            //    config.EnableDetailedErrors = true;
            //}
            
            //config.EnableCrossDomain = false;


            //RouteTable.Routes.MapHubs(config);
        }

      

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}