using System;
using System.Diagnostics;

namespace Zbang.Cloudents.MobileApp2
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Trace.TraceInformation("Starting service");
            WebApiConfig.Register();
        }
        
    }
}