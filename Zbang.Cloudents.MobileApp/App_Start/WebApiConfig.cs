﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using Autofac;
using Microsoft.WindowsAzure.Mobile.Service.Config;
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
            options.LoginProviders.Add(typeof (CustomLoginProvider));

            //Microsoft.WindowsAzure.Mobile.Service.Config.StartupOwinAppBuilder.Initialize(appBuilder2 =>
            //{

            //    Zbox.Infrastructure.Security.Startup.ConfigureAuth(appBuilder2, false);
            //    //    //Configure OWIN here
            //    //    //appBuilder.UseFacebookAuthentication("", "");
            //});

            //Microsoft.WindowsAzure.Mobile.Service.Config.StartupOwinAppBuilder.Initialize(Zbox.Infrastructure.Security.Startup.ConfigureAuth);

            var builder = new ConfigBuilder(options, ConfigureDependencies);
            // Use this class to set WebAPI configuration options

            //HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
            HttpConfiguration config = ServiceConfig.Initialize(builder);
            //HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options, ConfigureDependencies));





            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Default;
             

            
            //Database.SetInitializer(new MobileServiceInitializer());
        }


        private static void ConfigureDependencies(HttpConfiguration configuration, ContainerBuilder builder)
        {
            // Configure DI here

            // Register our custom builder
            //var instance = new ServiceInitialize(configuration);
            //builder.RegisterType<ServiceInitialize>().As<IOwinAppBuilderExtension>();
            //builder.RegisterInstance(instance).As<IOwinAppBuilder>();

            IocFactory.Unity.ContainerBuilder = builder;
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            //Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();

            //configuration.EnsureInitialized();
            //Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
        }
    }
}

