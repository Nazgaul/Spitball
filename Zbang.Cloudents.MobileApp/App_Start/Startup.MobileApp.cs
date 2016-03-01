﻿using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Autofac;
using Zbang.Zbox.Infrastructure.Security;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Autofac.Integration.WebApi;
using System.Reflection;

namespace Zbang.Cloudents.MobileApp
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();


            //var isoSettings = config.Formatters.JsonFormatter.SerializerSettings.Converters.OfType<IsoDateTimeConverter>().Single();
            //isoSettings.DateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'";


            new MobileAppConfiguration()
               .UseDefaultConfiguration()
               .ApplyTo(config);


            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            //if (string.IsNullOrEmpty(settings.HostName))
            //{
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            //}
            ConfigureDependencies(app, config);
           
            app.UseWebApi(config);
            
            //config.Routes.MapHttpRoute("CustomAuth", ".auth/login/CustomAuth", new { controller = "CustomAuth" });
        }

        private static void ConfigureDependencies(IAppBuilder app, HttpConfiguration config)
        {
            var builder = IocFactory.IocWrapper.ContainerBuilder;
            //IocFactory.IocWrapper.ContainerBuilder = builder;
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();

            builder.RegisterType<SeachConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
                .WithParameter("isDevelop", false)
                .InstancePerLifetimeScope();

            //builder.RegisterType<SendPush>()
            //.As<ISendPush>()
            //.WithParameter("connectionString", ConfigFetcher.Fetch("MS_NotificationHubConnectionString"))
            //.WithParameter("hubName", ConfigFetcher.Fetch("MS_NotificationHubName"))
            //.InstancePerLifetimeScope();
            RegisterIoc.Register();

            var x = new ApplicationDbContext("Zbox");

            builder.Register(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.IocWrapper.ContainerBuilder.Register(
               c => HttpContext.Current.GetOwinContext().Authentication);

            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();

            //Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            var container = IocFactory.IocWrapper.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);

        }
    }


}

