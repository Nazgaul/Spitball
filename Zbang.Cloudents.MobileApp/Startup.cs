using System.Diagnostics;
using Microsoft.ApplicationInsights.Channel;
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
            TelemetryConfiguration.Active.InstrumentationKey = "7f8df0c1-018b-4f0c-95bc-0441481acf0a";
            Trace.TraceInformation("Starting service");
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, false); //need for forgot password
            ConfigureMobileApp(app);
        }

        public class TelemetryInitializer : ITelemetryInitializer
        {
            public void Initialize(ITelemetry telemetry)
            {
                //telemetry.Context.Cloud.
                telemetry.Context.Properties["environment"] = "MobileApp";
            }
        }
    }
}