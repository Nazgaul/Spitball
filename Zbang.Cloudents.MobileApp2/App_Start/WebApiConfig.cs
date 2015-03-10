using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Autofac;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure.Mobile.Service;
using Newtonsoft.Json.Converters;
using Zbang.Cloudents.MobileApp2.Models;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;

namespace Zbang.Cloudents.MobileApp2
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            var options = new ConfigOptions();
            options.LoginProviders.Add(typeof(CustomLoginProvider));
            options.PushAuthorization =
                Microsoft.WindowsAzure.Mobile.Service.Security.AuthorizationLevel.User;

            var builder = new ConfigBuilder(options, ConfigureDependencies);
            // Use this class to set WebAPI configuration options

            //HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));
            HttpConfiguration config = ServiceConfig.Initialize(builder);
            var isoSettings = config.Formatters.JsonFormatter.SerializerSettings.Converters.OfType<IsoDateTimeConverter>().Single();
            isoSettings.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";
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
            //builder.RegisterType<PushNotification>().As<IPushNotification>();
            IocFactory.IocWrapper.ContainerBuilder = builder;
            Zbox.Infrastructure.RegisterIoc.Register();

            builder.RegisterType<SeachConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName",ConfigFetcher.Fetch("AzureSeachServiceName"))
                .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
                .WithParameter("isDevelop", true)
                .InstancePerLifetimeScope();

            builder.RegisterType<SendPush>()
            .As<ISendPush>()
            .WithParameter("connectionString", ConfigFetcher.Fetch("MS_NotificationHubConnectionString"))
            .WithParameter("hubName", ConfigFetcher.Fetch("MS_NotificationHubName"))
            .InstancePerLifetimeScope();
            RegisterIoc.Register();

            var x = new ApplicationDbContext();
            builder.Register(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.IocWrapper.ContainerBuilder.Register(
               c => HttpContext.Current.GetOwinContext().Authentication);
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.StorageApp.RegisterIoc.Register();

            //Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();

            
            //configuration.EnsureInitialized();

        }
    }
}