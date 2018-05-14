using System;
using System.Linq;
using System.Reflection;
using Autofac;
using AutoMapper;
using Cloudents.Core.Attributes;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
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
            builder.Register(c => new MapperConfiguration(cfg =>
            {
                //cfg.ConstructServicesUsing(c.Resolve);
                //foreach (var profile in profiles)
                //{
                //    cfg.AddProfile(profile);
                //}
                cfg.AddProfiles(assembly);
            })).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve<IComponentContext>().Resolve))
                .As<IMapper>().InstancePerLifetimeScope();
        }
    }
   
}