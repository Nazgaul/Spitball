using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(Zbang.Cloudents.Mvc2Jared.Startup))]

namespace Zbang.Cloudents.Mvc2Jared
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);
            IocConfig.RegisterTypes(app);
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
