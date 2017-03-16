using Autofac;
using Autofac.Integration.Mvc;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zbang.Zbox.Infrastructure;
using Zbang.Zbox.Infrastructure.Ioc;
using Zbang.Zbox.ReadServices;

namespace Zbang.Cloudents.Mvc2Jared
{
    public static class IocConfig
    {
        public static void RegisterTypes(IAppBuilder app)
        {

                var builder = IocFactory.IocWrapper.ContainerBuilder;
            builder.RegisterModule<InfrastructureModule>();

            builder.RegisterModule<ReadServiceModule>();


                builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
                builder.RegisterFilterProvider();


                var container = IocFactory.IocWrapper.Build();
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

                app.UseAutofacMiddleware(container);
                app.UseAutofacMvc();
        }
    }
}