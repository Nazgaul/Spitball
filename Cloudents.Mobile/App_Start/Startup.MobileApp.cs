using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.Framework;
using Cloudents.Mobile.Filters;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json;
using Owin;

namespace Cloudents.Mobile
{
    public partial class Startup
    {
        /// <summary>
        /// Entry point of app
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            var config = new HttpConfiguration();
            //config.EnableSystemDiagnosticsTracing();
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            new MobileAppConfiguration()
        .AddMobileAppHomeController()             // from the Home package
        .MapApiControllers()
        //   .AddPushNotifications()                   // from the Notifications package
        .ApplyTo(config);

            //config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
            var settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

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
            var keys = new ConfigurationKeys
            {
                Db = ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                Search = new SearchServiceCredentials(ConfigurationManager.AppSettings["AzureSearchServiceName"],
                    ConfigurationManager.AppSettings["AzureSearchKey"]),
                Redis = ConfigurationManager.AppSettings["Redis"],
                Storage = ConfigurationManager.AppSettings["Storage"],
                SystemUrl = ConfigurationManager.AppSettings["SystemUrl"]
            };
            builder.Register(c => keys).As<IConfigurationKeys>();
            builder.RegisterModule<ModuleMobile>();
            builder.RegisterModule<ModuleDb>();

            builder.RegisterWebApiModelBinderProvider();
            builder.RegisterType<LocationEntityBinder>().AsModelBinderForTypes(typeof(Location));
            builder.RegisterType<GeoPointEntityBinder>().AsModelBinderForTypes(typeof(GeoPoint));
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //ConfigureSignalR(app, container);
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            app.UseWebApi(config);
            ConfigureSwagger(config);
        }
    }
}

