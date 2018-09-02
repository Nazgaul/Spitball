using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Database.Query;
using Cloudents.Infrastructure.Database.Repositories;
using JetBrains.Annotations;
using NHibernate;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Database
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.Function)]
    [ModuleRegistration(Core.Enum.System.Admin)]
    [UsedImplicitly]
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>().SingleInstance();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession())
                .InstancePerLifetimeScope();

            builder.RegisterType<ReadonlySession>().InstancePerLifetimeScope();
            builder.RegisterType<ReadonlyStatelessSession>().InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
            builder.RegisterType<QueryBuilder>().AsSelf().SingleInstance();
            builder.RegisterGenericDecorator(
                typeof(CacheQueryHandlerDecorator<,>),
                typeof(IQueryHandler<,>),
                fromKey: "handler");

            builder.RegisterAssemblyTypes(assembly).As(o => o.GetInterfaces()
                .Where(i => i.IsClosedTypeOf(typeof(IQueryHandler<,>)))
                .Select(i => new KeyedService("handler", i)));
            base.Load(builder);
        }
    }

    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Function)]
    [UsedImplicitly]
    public class ModuleMailGun : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactoryMailGun>().SingleInstance();
            builder.Register(c => c.Resolve<UnitOfWorkFactoryMailGun>().OpenSession()).Keyed<ISession>(Core.Enum.Database.MailGun).InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            builder.Register(c =>
                {
                    var session = c.ResolveKeyed<ISession>(Core.Enum.Database.MailGun);
                    return new UnitOfWork(session);
                })
                .Keyed<IUnitOfWork>(Core.Enum.Database.MailGun).InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var session = c.ResolveKeyed<ISession>(Core.Enum.Database.MailGun);
                return new MailGunStudentRepository(session);
            }).AsImplementedInterfaces();
        }
    }
}