using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
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
            //var config = new HubConfiguration();

            // Register your SignalR hubs.

            var builder = IocFactory.IocWrapper.ContainerBuilder;

            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
            // Set the dependency resolver to be Autofac.
            var container = IocFactory.IocWrapper.Build();
            //config.Resolver = new AutofacDependencyResolver(container);

            // OWIN SIGNALR SETUP:

            // Register the Autofac middleware FIRST, then the standard SignalR middleware.
            //app.UseAutofacMiddleware(container);
            //app.MapSignalR("/signalr", config);

            // To add custom HubPipeline modules, you have to get the HubPipeline
            // from the dependency resolver, for example:
            //var hubPipeline = config.Resolver.Resolve<IHubPipeline>();
            //hubPipeline.AddModule(new MyPipelineModule());

            app.UseCors(CorsOptions.AllowAll);

            GlobalHost.DependencyResolver.Register(typeof(IJavaScriptMinifier), () => new MimifyProxy());
            //GlobalHost.DependencyResolver.Register(typeof(IJavaScriptProxyGenerator), () => new x());
            //GlobalHost.DependencyResolver.UseServiceBus("Endpoint=sb://cloudentsmsg-ns.servicebus.windows.net/;SharedAccessKeyName=signalr;SharedAccessKey=lyODoV4e3aapUw9tm8i6jvxSF5GT4w+Raj8ENlBHyUE=;EntityPath=signal-r", "signalr");
            //app.Map("", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);
            //    map.RunSignalR(new HubConfiguration
            //    {
            //        EnableDetailedErrors = true,
            //        EnableJSONP = true

            //    });
            //});
            app.UseAutofacMiddleware(container);
            Zbox.Infrastructure.Security.Startup.ConfigureAuth(app, true);
            app.MapSignalR("/s", new HubConfiguration
            {
                EnableDetailedErrors = true,
                Resolver = new AutofacDependencyResolver(container)
            });
            GlobalHost.HubPipeline.RequireAuthentication();
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
