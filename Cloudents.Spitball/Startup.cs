using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Cloudents.Spitball.Startup))]

namespace Cloudents.Spitball
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            var builder = new ContainerBuilder();

            //builder.RegisterModule<InfrastructureModule>();

            //builder.RegisterModule<DataModule>();
            //builder.RegisterModule<FileModule>();
            //builder.RegisterModule<StorageModule>();
            //builder.RegisterModule<MailModule>();


            //builder.RegisterModule<SearchModule>();

            //var x = new ApplicationDbContext(ConfigFetcher.Fetch("Zbox"));
            //builder.Register(c => x).AsSelf().InstancePerRequest();
            //builder.RegisterType<ApplicationUserManager>().AsSelf().InstancePerRequest().As<IAccountService>().InstancePerRequest();

            //try
            //{
            //    builder.Register(c => new UserStore<ApplicationUser>(x))
            //        .AsImplementedInterfaces().InstancePerRequest();
            //}
            //catch (Exception ex)
            //{
            //    TraceLog.WriteError(ex);
            //}

            //IocFactory.IocWrapper.ContainerBuilder.Register(
            //    c => HttpContext.Current.GetOwinContext().Authentication);

            //builder.RegisterModule<WriteServiceModule>();
            //builder.RegisterModule<ReadServiceModule>();

            //builder.RegisterModule<CommandsModule>();

            //builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            //builder.RegisterFilterProvider();

            ////builder.RegisterModule<AiModule>();
            //builder.RegisterModule<AutofacWebTypesModule>();

            //builder.RegisterType<CookieHelper>().As<ICookieHelper>();
            //builder.RegisterType<LanguageCookieHelper>().As<ILanguageCookieHelper>();
            //builder.RegisterType<LanguageMiddleware>().InstancePerRequest();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //we need that for blob getting the blob container url
            //DependencyResolver.Current.GetService<IBlobProvider>();
            //DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();

            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
