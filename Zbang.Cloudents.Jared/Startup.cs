﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Zbang.Cloudents.Jared.Startup))]

namespace Zbang.Cloudents.Jared
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureMobileApp(app);
        }
    }
}