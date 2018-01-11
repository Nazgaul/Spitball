using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Cloudents.Mobile.Startup))]

namespace Cloudents.Mobile
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}