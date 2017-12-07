using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Autofac;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json;
using Owin;
using Zbang.Cloudents.Connect;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.DataAccess;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;
using Zbang.Zbox.WorkerRoleSearch;
using t = Cloudents.Infrastructure;

namespace Zbang.Cloudents.Jared
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            var config = new HttpConfiguration();
            config.EnableSystemDiagnosticsTracing();
            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            new MobileAppConfiguration()
        .AddMobileAppHomeController()             // from the Home package
        .MapApiControllers()
       //   .AddPushNotifications()                   // from the Notifications package
        .ApplyTo(config);

            config.Services.Add(typeof(IExceptionLogger), new AiExceptionLogger());
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
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<CommandsModule>();
            builder.RegisterModule<WriteServiceModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<ReadServiceModule>();
            builder.RegisterModule<MailModule>();
            var module = new t.InfrastructureModule(
                ConfigurationManager.ConnectionStrings["ZBox"].ConnectionString,
                ConfigurationManager.AppSettings["AzureSearchServiceName"],
                ConfigurationManager.AppSettings["AzureSearchKey"],
                ConfigurationManager.AppSettings["Redis"], t.Environment.Mobile);
            builder.RegisterModule(module);

            builder.RegisterType<JaredSendPush>()
                .As<IJaredPushNotification>()
                .WithParameter("connectionString", "Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=")
                .WithParameter("hubName", "jared-spitball")
                .InstancePerLifetimeScope();
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterType<TelemetryLogger>().As<ILogger>();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            ConfigureSignalR(app, container);
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);

            app.UseWebApi(config);
            ConfigureSwagger(config);
        }

        private static void ConfigureSignalR(IAppBuilder app, IContainer container)
        {
            var config = new HubConfiguration
            {
                EnableDetailedErrors = false
               // Resolver = new AutofacDependencyResolver(container)
            };

            GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);// config.Resolver;
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new UserIdProvider());

            GlobalHost.DependencyResolver.UseServiceBus(
                ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"], "signalr");
            app.MapSignalR(config);
        }
    }
}

