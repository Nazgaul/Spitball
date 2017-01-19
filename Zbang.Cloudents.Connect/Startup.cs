using System.Reflection;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Transports;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Ioc;

[assembly: OwinStartup(typeof(Zbang.Cloudents.Connect.Startup))]

namespace Zbang.Cloudents.Connect
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //var builder = new ContainerBuilder();

            // STANDARD SIGNALR SETUP:

            // Get your HubConfiguration. In OWIN, you'll create one
            // rather than using GlobalHost.
            var config = new HubConfiguration
            {
                EnableDetailedErrors = true
            };

            // Register your SignalR hubs.

            var builder = IocFactory.IocWrapper.ContainerBuilder;

            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterModule<InfrastructureModule>();

            //Zbox.Infrastructure.Data.RegisterIoc.Register();
            builder.RegisterModule<DataModule>();

            builder.RegisterModule<StorageModule>();
            builder.RegisterModule<WriteServiceModule>();
            //Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            Zbang.Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            builder.RegisterModule<CommandsModule>();
            //Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            // Set the dependency resolver to be Autofac.
            var container = IocFactory.IocWrapper.Build();
            config.Resolver = new AutofacDependencyResolver(container);

            


            GlobalHost.DependencyResolver = config.Resolver;
            GlobalHost.DependencyResolver.Register(typeof(IJavaScriptMinifier), () => new MimifyProxy());
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new UserIdProvider());
            if (Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable)
            {
                GlobalHost.DependencyResolver.UseServiceBus(
                    "Endpoint=sb://cloudentsmsg-ns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oePM1T/GBe2ZlaDhik3MLHNXstsM4lhnCTyRTBi0bmQ=",
                    "signalr");
            }
            
            app.UseCors(CorsOptions.AllowAll);
            app.UseAutofacMiddleware(container);
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);

            var heartBeat = GlobalHost.DependencyResolver.Resolve<ITransportHeartbeat>();
            var writeService = GlobalHost.DependencyResolver.Resolve<IZboxWriteService>();

            //TODO: wire to autofac
            var monitor = new PresenceMonitor(heartBeat, writeService);
            monitor.StartMonitoring();
            app.MapSignalR("/s", config);
        }
    }
}
