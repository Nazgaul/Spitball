using Microsoft.Owin;
using Owin;

namespace Zbang.Cloudents.Mobile
{
    [assembly: OwinStartup(typeof(Zbang.Cloudents.Mobile.Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);

        }
    }
}