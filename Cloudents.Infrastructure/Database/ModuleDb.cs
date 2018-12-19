using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Cloudents.Application.Attributes;
using Cloudents.Application.Interfaces;
using Cloudents.Infrastructure.Database.Query;
using Cloudents.Infrastructure.Database.Repositories;
using JetBrains.Annotations;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure.Database
{
    [ModuleRegistration(Application.Enum.System.Console)]
    [ModuleRegistration(Application.Enum.System.Web)]
    [ModuleRegistration(Application.Enum.System.Function)]
    [ModuleRegistration(Application.Enum.System.Admin)]
    [UsedImplicitly]
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>().SingleInstance();
            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession())
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();


            builder.RegisterType<QuerySession>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>)).AsSelf()
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterType<QueryBuilder>().AsSelf();
            

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

    //[ModuleRegistration(Core.Enum.System.Console)]
    //[ModuleRegistration(Core.Enum.System.MailGun)]
    //[UsedImplicitly]
    //public class ModuleMailGun : Module
    //{
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        builder.RegisterType<UnitOfWorkFactoryMailGun>().SingleInstance();
    //        builder.Register(c => c.Resolve<UnitOfWorkFactoryMailGun>().OpenSession()).InstancePerLifetimeScope();

    //        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
    //        builder.RegisterType<MailGunStudentRepository>().AsImplementedInterfaces();
    //    }
    //}
}