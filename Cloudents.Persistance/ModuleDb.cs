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
       // private readonly string _dbConnectionString;
       // private readonly string _redisConnectionString;

        //public ModuleDb(string dbConnectionString, string redisConnectionString)
        //{
        //    _dbConnectionString = dbConnectionString;
        //    _redisConnectionString = redisConnectionString;
        //}
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

            //builder.RegisterType<QueryBuilder>().AsSelf();
            builder.RegisterType<QuerySession>().InstancePerLifetimeScope();


            //builder.RegisterGenericDecorator(
            //    typeof(CacheQueryHandlerDecorator<,>),
            //    typeof(IQueryHandler<,>),
            //    fromKey: "handler");

            //builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
            //    .Where(i => i.IsClosedTypeOf(typeof(IQueryHandler<,>)))
            //    .Select(i => new KeyedService("handler", i)));
            base.Load(builder);
        }
    }
}