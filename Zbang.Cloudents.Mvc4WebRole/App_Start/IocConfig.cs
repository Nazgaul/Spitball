using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.CommandHandlers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.File;
using Zbang.Zbox.Infrastructure.Mail;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc4WebRole
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class IocConfig
    {
        private static readonly object ThisLock = new object();
        public static void RegisterTypes(IAppBuilder app)
        {
            lock (ThisLock)
            {
                var builder = new ContainerBuilder();// IocFactory.IocWrapper.ContainerBuilder;

                builder.RegisterModule<InfrastructureModule>();

                builder.RegisterModule<DataModule>();
                builder.RegisterModule<FileModule>();
                builder.RegisterModule<StorageModule>();
                builder.RegisterModule<MailModule>();

                builder.RegisterModule<SearchModule>();

                var x = new ApplicationDbContext(ConfigFetcher.Fetch("Zbox"));
                builder.Register(c => x).AsSelf().InstancePerRequest();
                builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest().As<IAccountService>().InstancePerRequest();

                try
                {
                    builder.Register(c => new UserStore<ApplicationUser>(x))
                        .AsImplementedInterfaces().InstancePerRequest();
                }
                catch (Exception ex)
                {
                    TraceLog.WriteError(ex);
                }

                builder.Register(
                    c => HttpContext.Current.GetOwinContext().Authentication);

                builder.RegisterModule<WriteServiceModule>();
                builder.RegisterModule<ReadServiceModule>();

                builder.RegisterModule<CommandsModule>();

                builder.RegisterControllers(typeof (MvcApplication).Assembly).PropertiesAutowired();

                //builder.RegisterType<LandingPageAttribute>()

                builder.RegisterFilterProvider();

                //builder.RegisterModule<AiModule>();
                builder.RegisterModule<AutofacWebTypesModule>();

                builder.RegisterType<CookieHelper>().As<ICookieHelper>();
                builder.RegisterType<LanguageCookieHelper>().As<ILanguageCookieHelper>();
                builder.Register(c =>
                new LandingPageAttribute(c.Resolve<IZboxReadService>(), c.Resolve<ICookieHelper>())).AsActionFilterFor<Controller>();

                builder.RegisterType<LanguageMiddleware>().InstancePerRequest();
                var container = builder.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

                //we need that for blob getting the blob container url
                //DependencyResolver.Current.GetService<IBlobProvider>();
                DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();

                app.UseAutofacLifetimeScopeInjector(container);
                app.UseMiddlewareFromContainer<LanguageMiddleware>();
                //app.UseAutofacMiddleware(container);
                app.UseAutofacMvc();
            }
        }
    }
}
