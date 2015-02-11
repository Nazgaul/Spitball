using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Owin;

namespace Zbang.Cloudents.Mvc4WebRole
{
    [assembly: OwinStartup(typeof(Zbang.Cloudents.Mvc4WebRole.Startup))]
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app,true);

        }
    }
}