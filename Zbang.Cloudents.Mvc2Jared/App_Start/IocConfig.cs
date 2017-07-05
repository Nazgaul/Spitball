using Autofac;
using Autofac.Integration.Mvc;
using Owin;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Notifications;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc2Jared
{
    public static class IocConfig
    {
        public static void RegisterTypes(IAppBuilder app)
        {

                var builder = IocFactory.IocWrapper.ContainerBuilder;
            builder.RegisterModule<InfrastructureModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<FileModule>();
            builder.RegisterModule<StorageModule>();
            IocFactory.IocWrapper.ContainerBuilder.Register(
                    c => HttpContext.Current.GetOwinContext().Authentication);
            builder.RegisterModule<WriteServiceModule>();
            builder.RegisterModule<ReadServiceModule>();
            builder.RegisterModule<CommandsModule>();


            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
                builder.RegisterFilterProvider();
            builder.RegisterType<JaredSendPush>()
                .As<IJaredPushNotification>()
                .WithParameter("connectionString", "Endpoint=sb://spitball.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=1+AAf2FSzauWHpYhHaoweYT9576paNgmicNSv6jAvKk=")
                .WithParameter("hubName", "jared-spitball")
                .InstancePerLifetimeScope();

            var container = IocFactory.IocWrapper.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();
                app.UseAutofacMiddleware(container);
                app.UseAutofacMvc();
        }
    }
}