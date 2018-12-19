using System.Reflection;
using Autofac;
using AutoMapper;
using Cloudents.Application.Attributes;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Mapper
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.WorkerRole)]
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
        }
    }
}