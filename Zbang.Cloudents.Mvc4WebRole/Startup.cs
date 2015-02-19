using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.Mvc4WebRole
{
    [assembly: OwinStartup(typeof(Zbang.Cloudents.Mvc4WebRole.Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            UnityConfig.RegisterTypes(app);
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);
            //app.CreatePerOwinContext(ApplicationDbContext.Create);

            app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<ApplicationUserManager>());
            

        }
    }
}