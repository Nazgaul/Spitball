using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Domain.Services;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ai;
using Zbang.Zbox.Infrastructure.Azure;
using Zbang.Zbox.Infrastructure.Data;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Trace;

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
                var builder = IocFactory.IocWrapper.ContainerBuilder;

                builder.RegisterModule<InfrastructureModule>();

                //Zbox.Infrastructure.RegisterIoc.Register();
                builder.RegisterModule<DataModule>();
                //Zbox.Infrastructure.Data.RegisterIoc.Register();
                Zbox.Infrastructure.File.RegisterIoc.Register();
                builder.RegisterModule<StorageModule>();
                //Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();
                Zbox.Infrastructure.Mail.RegisterIoc.Register();



                //Zbox.Infrastructure.Ai.RegisterIoc.Register();
                //builder.RegisterType<SeachConnection>()
                //    .As<ISearchConnection>()
                //    .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                //    .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
                //    .InstancePerLifetimeScope();
                builder.RegisterModule<SearchModule>();
                //RegisterIoc.Register();

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

                IocFactory.IocWrapper.ContainerBuilder.Register(
                    c => HttpContext.Current.GetOwinContext().Authentication);

                builder.RegisterModule<WriteServiceModule>();
                //Zbox.Domain.Services.RegisterIoc.Register();

                Zbox.ReadServices.RegisterIoc.Register();
                Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

                builder.RegisterControllers(typeof (MvcApplication).Assembly).PropertiesAutowired();
                builder.RegisterFilterProvider();


                builder.RegisterModule<AiModule>();
                builder.RegisterModule<AutofacWebTypesModule>();
                //builder.RegisterModule<AutofacWebTypesModule>();

                builder.RegisterType<CookieHelper>().As<ICookieHelper>();
                builder.RegisterType<LanguageCookieHelper>().As<ILanguageCookieHelper>();
                builder.RegisterType<LanguageMiddleware>().InstancePerRequest();

                var container = IocFactory.IocWrapper.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


                //we need that for blob getting the blob container url
                //DependencyResolver.Current.GetService<IBlobProvider>();
                DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();

                app.UseAutofacMiddleware(container);
                app.UseAutofacMvc();
            }
        }
    }
}
