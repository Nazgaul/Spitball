using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.WebApi;
using Cloudents.Infrastructure;
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

            var module = new MobileAppInfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"],
                ConfigurationManager.AppSettings["Redis"],
                null);
            builder.RegisterModule(module);

            //builder.RegisterType<JaredSendPush>()
            //    .As<IJaredPushNotification>()
            //    .WithParameter("connectionString", "Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=")
            //    .WithParameter("hubName", "jared-spitball")
            //    .InstancePerLifetimeScope();
            // builder.RegisterHubs(Assembly.GetExecutingAssembly());
            //builder.RegisterType<TelemetryLogger>().As<ILogger>();
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

