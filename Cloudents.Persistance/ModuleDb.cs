using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Persistence.Repositories;
using Cloudents.Query;
using NHibernate;
using System.Reflection;
using Module = Autofac.Module;

namespace Cloudents.Persistence
{

    public class ModuleDb : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>()
                .SingleInstance().As<IStartable>();

            builder.RegisterType<PublishEventsListener>().AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession()).InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();


            builder.RegisterType<LoggingInterceptor>().As<IInterceptor>();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces().InstancePerDependency();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>))
                .AsSelf()
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<QuerySession>().InstancePerDependency();

            builder.RegisterType<DapperRepository>().As<IDapperRepository>().AsSelf();

            base.Load(builder);
        }
    }
}