using System.Reflection;
using Autofac;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data.Query;
using Cloudents.Infrastructure.Data.Repositories;
using JetBrains.Annotations;
using NHibernate;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Data
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.Function)]
    [UsedImplicitly]
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>().SingleInstance();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession())
                .InstancePerLifetimeScope();


            builder.RegisterType<ReadonlySession>();
            builder.RegisterType<ReadonlyStatelessSession>();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>)).AsSelf().AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(IQueryHandler<,>));
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
            builder.Register(c => c.Resolve<UnitOfWorkFactoryMailGun>().OpenSession()).Keyed<ISession>(Database.MailGun).InstancePerLifetimeScope();

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            builder.Register(c => new UnitOfWork(c.ResolveKeyed<ISession>(Database.MailGun)))
                .Keyed<IUnitOfWork>(Database.MailGun).InstancePerLifetimeScope();

            builder.Register(c =>
            {
                var session = c.ResolveKeyed<ISession>(Database.MailGun);
                return new MailGunStudentRepository(session);
            }).AsImplementedInterfaces();
        }
    }
}