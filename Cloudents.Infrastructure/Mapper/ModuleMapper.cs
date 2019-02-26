//using System.Reflection;
//using Autofac;
//using AutoMapper;
//using JetBrains.Annotations;
//using Module = Autofac.Module;

//namespace Cloudents.Infrastructure.Mapper
//{
//    [UsedImplicitly]
//    public class ModuleMapper : Module
//    {
//        protected override void Load(ContainerBuilder builder)
//        {
//            var assembly = Assembly.GetExecutingAssembly();
//            builder.Register(_ => new MapperConfiguration(cfg =>
//            {
//                cfg.AddProfiles(assembly);
//            })).AsSelf().SingleInstance();

//            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper(c.Resolve<IComponentContext>().Resolve))
//                .As<IMapper>().InstancePerLifetimeScope();
//        }
//    }
//}