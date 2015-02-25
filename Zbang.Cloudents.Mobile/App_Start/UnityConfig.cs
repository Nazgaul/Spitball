using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Search;
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
            var builder = IocFactory.Unity.ContainerBuilder;

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

            var container = IocFactory.Unity.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //iocContainer.RegisterType<IUserProfile, UserProfile>();


            //we need that for blob getting the blob container url
            DependencyResolver.Current.GetService<IBlobProvider>();
            DependencyResolver.Current.GetService<ISearchConnection>();
            DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();


        }
    }
}
