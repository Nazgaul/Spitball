using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;

namespace Zbang.Cloudents.Mobile
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {

        public static void RegisterTypes()
        {
            var builder = IocFactory.IocWrapper.ContainerBuilder;

            Zbox.Infrastructure.RegisterIoc.Register();
            Zbox.Infrastructure.Data.RegisterIoc.Register();
            Zbox.Infrastructure.File.RegisterIoc.Register();
            Zbox.Infrastructure.Azure.Ioc.RegisterIoc.Register();

            builder.RegisterType<SeachConnection>()
              .As<ISearchConnection>()
              .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
              .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey"))
              .InstancePerLifetimeScope();

            RegisterIoc.Register();
            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterFilterProvider();

            var x = new ApplicationDbContext(ConfigFetcher.Fetch("Zbox"));
            builder.Register(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.IocWrapper.ContainerBuilder.Register(
               c => HttpContext.Current.GetOwinContext().Authentication);

            var container = IocFactory.IocWrapper.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //iocContainer.RegisterType<IUserProfile, UserProfile>();


            //we need that for blob getting the blob container url
            DependencyResolver.Current.GetService<IBlobProvider>();
            DependencyResolver.Current.GetService<ISearchConnection>();
            DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();


        }
    }
}
