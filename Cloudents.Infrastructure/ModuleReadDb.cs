//using System.Reflection;
//using Autofac;
//using Cloudents.Core.Interfaces;
//using Cloudents.Query;
//using Module = Autofac.Module;

//namespace Cloudents.Infrastructure
//{
//    public class ModuleReadDb : Module
//    {
//        protected override void Load(ContainerBuilder builder)
//        {
//            var currentAssembly = Assembly.GetExecutingAssembly();
//            builder.RegisterType<DapperRepository>().AsSelf();
//            //builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
//            //builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));
//        }
//    }
//}