using System.Configuration;
using System.Web.Http;
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
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Azure.Mobile.Server.Tables.Config;
using Zbang.Cloudents.Connect;
using Zbang.Cloudents.MobileApp.Controllers;
using Zbang.Cloudents.MobileApp.Extensions;
using Zbang.Cloudents.MobileApp.Filters;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.MobileApp
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {

            // Register your SignalR hubs.
            IocFactory.IocWrapper.ContainerBuilder.RegisterHubs(Assembly.GetExecutingAssembly());
            var config = new HttpConfiguration();
            app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
            {
                // This middleware is intended to be used locally for debugging. By default, HostName will
                // only have a value when running in an App Service application.
                SigningKey = CustomLoginController.GetEnvironmentVariableAuth(),// Environment.GetEnvironmentVariable("WEBSITE_AUTH_SIGNING_KEY"),
                ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                TokenHandler = config.GetAppServiceTokenHandler()
            });

            IocFactory.IocWrapper.ContainerBuilder.RegisterWebApiFilterProvider(config);
            //var builder = IocFactory.IocWrapper.ContainerBuilder;
            var container = ConfigureDependencies();

            ConfigureSignalR(app, container);
            config.MapHttpAttributeRoutes();

            new MobileAppConfiguration()
                .AddMobileAppHomeController()             // from the Home package
                .MapApiControllers()
                .AddTables(                               // from the Tables package
                new MobileAppTableConfiguration()
                    .MapTableControllers()
                    //.AddEntityFramework()             // from the Entity package
                )
            //.AddPushNotifications()                   // from the Notifications package
            .MapLegacyCrossDomainController()         // from the CrossDomain package
            .ApplyTo(config);


            


            config.Filters.Add(new JsonSerializeAttribute());
            config.Filters.Add(new UnhandledExceptionFilter());

            //config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);
            app.UseWebApi(config);
        }

        private static void ConfigureSignalR(IAppBuilder app, IContainer container)
        {
            var config = new HubConfiguration
            {
                EnableDetailedErrors = false,
                Resolver = new AutofacDependencyResolver(container)
            };



            GlobalHost.DependencyResolver = config.Resolver;
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new UserIdProvider());

            GlobalHost.DependencyResolver.UseServiceBus(
                "Endpoint=sb://cloudentsmsg-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oePM1T/GBe2ZlaDhik3MLHNXstsM4lhnCTyRTBi0bmQ=",
                "signalr");
            app.UseAutofacMiddleware(container);
            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            var writeService = GlobalHost.DependencyResolver.Resolve<IZboxWriteService>();

            var monitor = new PresenceMonitor(heartBeat, writeService);
            monitor.StartMonitoring();
            app.MapSignalR(config);
        }

        private static IContainer ConfigureDependencies(/*IAppBuilder app, HttpConfiguration config*/)
        {
            var builder = IocFactory.IocWrapper.ContainerBuilder;
            //IocFactory.IocWrapper.ContainerBuilder = builder;
            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();

            builder.RegisterType<SeachConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
                .InstancePerLifetimeScope();

            RegisterIoc.Register();

            var x = new ApplicationDbContext("Zbox");
            builder.Register(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.IocWrapper.ContainerBuilder.Register(
               c => HttpContext.Current.GetOwinContext().Authentication);

            Zbox.Infrastructure.Data.RegisterIoc.Register();
            builder.RegisterModule<StorageModule>();
           // Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbox.Infrastructure.Mail.RegisterIoc.Register();
            //Zbox.Infrastructure.File.RegisterIoc.Register();
            builder.RegisterModule<WriteServiceModule>();
            //Zbox.Domain.Services.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

           
            return IocFactory.IocWrapper.Build();


        }
    }


}

