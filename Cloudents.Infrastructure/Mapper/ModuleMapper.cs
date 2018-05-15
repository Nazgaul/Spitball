using System.Reflection;
using Autofac;
using AutoMapper;
using Cloudents.Core.Attributes;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Mapper
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    //[ModuleRegistration(Core.Enum.System.Api)]
    //[ModuleRegistration(Core.Enum.System.Web)]
    [UsedImplicitly]
    public class ModuleMapper : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
          

            var assembly = Assembly.GetExecutingAssembly();
            builder.Register(_ => new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(assembly);
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve<IComponentContext>().Resolve))
                .As<IMapper>().InstancePerLifetimeScope();


            builder.RegisterType<Mapper>().AsImplementedInterfaces();
        }
    }
}