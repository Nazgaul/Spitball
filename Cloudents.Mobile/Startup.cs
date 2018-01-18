using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Cloudents.Mobile.Startup))]

namespace Cloudents.Mobile
{
    public partial class Startup
    {
        /// <summary>
        /// Start up point
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}