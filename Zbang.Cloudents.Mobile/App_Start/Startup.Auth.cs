using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Zbang.Cloudents.Mobile
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                CookieHttpOnly = true,
                CookieName = "cdAtest1",
                CookieSecure = CookieSecureOption.Always,
                SlidingExpiration = true,
                LoginPath = new Microsoft.Owin.PathString("/account/")
            });
        }
    }
}