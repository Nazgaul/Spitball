using System.Reflection;
using Autofac;
using Cloudents.Core.Interfaces;
using Module = Autofac.Module;

namespace Cloudents.Core
{
    public class ModuleCore : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(ICommandHandlerAsync<,>)).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(ICommandHandlerAsync<>)).AsImplementedInterfaces();
            builder.RegisterType<CommandBus>().As<ICommandBus>();

            builder.RegisterType<UrlConst>().As<IUrlBuilder>().SingleInstance();

            builder.RegisterType<UrlRedirectBuilder>().As<IUrlRedirectBuilder>();
            builder.RegisterType<Shuffle>().As<IShuffle>();
            //builder.RegisterGeneric(typeof(UrlRedirectBuilder<>)).As(typeof(IUrlRedirectBuilder<>));
        }
    }
}
