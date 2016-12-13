using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Zbang.Cloudents.MobileApp.Startup))]

namespace Zbang.Cloudents.MobileApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            TelemetryConfiguration.Active.InstrumentationKey = "f3425ae5-004c-4fb9-9999-0d59ba8d04fa";
            Trace.TraceInformation("Starting service");
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, false); //need for forgot password
            ConfigureMobileApp(app);
        }
    }
}