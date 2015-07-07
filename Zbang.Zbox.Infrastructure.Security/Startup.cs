﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Zbang.Zbox.Infrastructure.Security
{
    public class Startup
    {
        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }


        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public static void ConfigureAuth(IAppBuilder app, bool shouldUseCookie)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();
            
            
            //app.CreatePerOwinContext<UserManager>(UserManager.Create);
            //app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);


            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            if (shouldUseCookie)
            {
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    CookieDomain = "cloudents.com",
                    CookieName = "a1",
                    CookieSecure = CookieSecureOption.Always,
                    LogoutPath = new PathString("/account/logoff/"),
                    SlidingExpiration = true,
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/account/"),
                    Provider = new CookieAuthenticationProvider
                    {
                        OnApplyRedirect = ctx =>
                        {
                            if (!IsAjaxRequest(ctx.Request))
                            {
                                ctx.Response.Redirect(ctx.RedirectUri);
                            }
                        }
                        //OnResponseSignIn = ctx =>
                        //{
                            
                        //}
                       
                        //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager,User>()
                        //TimeSpan.FromDays(5))
                        // Enables the application to validate the security stamp when the user logs in.
                        // This is a security feature which is used when you change a password or add an external login to your account.  
                        //OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<UserManager, User>(
                        //    validateInterval: TimeSpan.FromMinutes(30),
                        //    regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager)),


                    }
                });
                //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

                // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
                //app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

                // Enables the application to remember the second login verification factor such as phone or email.
                // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
                // This is similar to the RememberMe option when you log in.
                //app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

                // Uncomment the following lines to enable logging in with third party login providers
                //app.UseMicrosoftAccountAuthentication(
                //    clientId: "",
                //    clientSecret: "");

                //app.UseTwitterAuthentication(
                //   consumerKey: "",
                //   consumerSecret: "");

                //app.UseFacebookAuthentication(
                //   appId: "",
                //   appSecret: "");

                //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
                //{
                //    ClientId = "",
                //    ClientSecret = ""
                //});
            }
        }
    }
}
