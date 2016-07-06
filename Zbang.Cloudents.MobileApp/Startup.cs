using System.Diagnostics;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Zbang.Cloudents.MobileApp.Startup))]

namespace Zbang.Cloudents.MobileApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Trace.TraceInformation("Starting service");
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, false); //need for forgot password
            ConfigureMobileApp(app);
        }
    }
}