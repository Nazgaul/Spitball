using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Autofac;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure.Mobile.Service;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.MobileApp2
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
            
            options.LoginProviders.Add(typeof(CustomLoginProvider));

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
            //config.SetIsHosted(true);


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
            builder.RegisterType<SeachConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName", "cloudents")
                .WithParameter("serviceKey", "5B0433BFBBE625C9D60F7330CFF103F0")
                .InstancePerLifetimeScope();
            RegisterIoc.Register();

            var x = new ApplicationDbContext();
            builder.Register(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.Unity.ContainerBuilder.Register(
               c => HttpContext.Current.GetOwinContext().Authentication);
            //Zbox.Infrastructure.Data.RegisterIoc.Register();
            //Zbox.Infrastructure.File.RegisterIoc.Register();
            //Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();

            //configuration.EnsureInitialized();
            //Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
        }
    }
}