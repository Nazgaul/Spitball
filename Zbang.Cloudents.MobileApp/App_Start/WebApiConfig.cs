using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using Zbang.Cloudents.MobileApp.DataObjects;
using Zbang.Cloudents.MobileApp.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Cloudents.MobileApp
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options, (configuration, builder) =>
            {
                IocFactory.Unity.ContainerBuilder = builder;
                Zbox.Infrastructure.RegisterIoc.Register();
                Zbox.Infrastructure.Data.RegisterIoc.Register();
                Zbox.Infrastructure.File.RegisterIoc.Register();
                Zbox.Domain.Services.RegisterIoc.Register();

                Zbox.ReadServices.RegisterIoc.Register();
                Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            }));

            //Microsoft.WindowsAzure.Mobile.Service.Config.StartupOwinAppBuilder.Initialize(Zbox.Infrastructure.Security.Startup.ConfigureAuth);


            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            //Database.SetInitializer(new MobileServiceInitializer());
        }
    }
}

