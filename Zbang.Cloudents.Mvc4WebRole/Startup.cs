using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.Mvc4WebRole
{
   // [assembly: OwinStartup(typeof(Zbang.Cloudents.Mvc4WebRole.Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.Use<LanguageMiddleware>();
            //app.Use((context, next) =>
            //{

            //    var cultureInfo = new CultureInfo("de-DE");
            //    Thread.CurrentThread.CurrentUICulture = cultureInfo;
            //    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
            //    return next.Invoke();
            //});
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);
            IocConfig.RegisterTypes(app);
            //app.CreatePerOwinContext(ApplicationDbContext.Create);

            //app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());


        }
    }
}