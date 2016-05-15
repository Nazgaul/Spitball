using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartup(typeof(Zbang.Cloudents.Connect.Startup))]

namespace Zbang.Cloudents.Connect
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            GlobalHost.DependencyResolver.Register(typeof(IJavaScriptMinifier), () => new MimifyProxy());
            //GlobalHost.DependencyResolver.Register(typeof(IJavaScriptProxyGenerator), () => new x());
            //GlobalHost.DependencyResolver.UseServiceBus("Endpoint=sb://cloudentsmsg-ns.servicebus.windows.net/;SharedAccessKeyName=signalr;SharedAccessKey=lyODoV4e3aapUw9tm8i6jvxSF5GT4w+Raj8ENlBHyUE=;EntityPath=signal-r", "signalr");
            //app.Map("", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);
            //    map.RunSignalR(new HubConfiguration
            //    {
            //        EnableDetailedErrors = true,
            //        EnableJSONP = true

            //    });
            //});
            app.MapSignalR("/s", new HubConfiguration
            {
                EnableDetailedErrors = true,
                // EnableJSONP = true
            });
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
