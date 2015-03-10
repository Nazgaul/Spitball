﻿using System;
using System.Web;
using System.Web.Mvc;
using Autofac.Builder;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Owin;
using Zbang.Cloudents.Mvc4WebRole.Helpers;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.Infrastructure.Search;
using Zbang.Zbox.Infrastructure.Security;
using Zbang.Zbox.Infrastructure.Storage;
using Zbang.Zbox.Infrastructure.Thumbnail;

namespace Zbang.Cloudents.Mvc4WebRole
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {



        public static void RegisterTypes(IAppBuilder app)
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

            Zbox.Infrastructure.Search.RegisterIoc.Register();

            var x = new ApplicationDbContext();
            builder.Register<ApplicationDbContext>(c => x).AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationUserManager>().AsSelf().As<IAccountService>().InstancePerLifetimeScope();

            builder.Register<UserStore<ApplicationUser>>(c => new UserStore<ApplicationUser>(x))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            IocFactory.IocWrapper.ContainerBuilder.Register<IAuthenticationManager>(
                c => HttpContext.Current.GetOwinContext().Authentication);


            Zbox.Domain.Services.RegisterIoc.Register();

            Zbox.ReadServices.RegisterIoc.Register();
            Zbox.Domain.CommandHandlers.Ioc.RegisterIoc.Register();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            builder.RegisterFilterProvider();
            var container = IocFactory.IocWrapper.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));


            //we need that for blob getting the blob container url
            DependencyResolver.Current.GetService<IBlobProvider>();

            DependencyResolver.Current.GetService<Zbox.Domain.Common.IZboxServiceBootStrapper>().BootStrapper();


            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();




        }
    }
}
