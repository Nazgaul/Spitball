using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Owin;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Jared
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            new MobileAppConfiguration()
        .AddMobileAppHomeController()             // from the Home package
        .MapApiControllers()

        .AddPushNotifications()                   // from the Notifications package
        .ApplyTo(config);


            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }


            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<CommandsModule>();
            builder.RegisterModule<WriteServiceModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<ReadServiceModule>();

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            app.UseWebApi(config);

        }
    }


}

