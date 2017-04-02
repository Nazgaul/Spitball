using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using System.Web.Helpers;
using System.Security.Claims;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(Zbang.Cloudents.Mvc2Jared.Startup))]

namespace Zbang.Cloudents.Mvc2Jared
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Enable the Application Sign In Cookie.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/")
            });
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);
            IocConfig.RegisterTypes(app);
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
