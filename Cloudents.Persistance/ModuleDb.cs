using System.Reflection;
using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Persistence.Repositories;
using Cloudents.Query;
using Module = Autofac.Module;

namespace Cloudents.Persistence
{
 
    public class ModuleDb : Module
    {
      
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>()
                .SingleInstance();

            builder.RegisterType<PublishEventsListener>().AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession())
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();


            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<QuerySession>().InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}