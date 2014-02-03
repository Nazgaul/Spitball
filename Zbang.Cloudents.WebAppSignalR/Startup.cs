using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(Zbang.Cloudents.WebAppSignalR.Startup))]
namespace Zbang.Cloudents.WebAppSignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR(new Microsoft.AspNet.SignalR.HubConfiguration
            {
                EnableJavaScriptProxies=false
            });
        }
    }
}