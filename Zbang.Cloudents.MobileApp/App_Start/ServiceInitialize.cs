using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.Config;
using Owin;

namespace Zbang.Cloudents.MobileApp
{
    public class ServiceInitialize : IOwinAppBuilderExtension
    {
        public void Configure(IAppBuilder appBuilder)
        {
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(appBuilder, false);
        }
    }


}