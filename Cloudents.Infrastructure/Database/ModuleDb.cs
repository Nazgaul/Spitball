using Autofac;
using Cloudents.Core;
using Cloudents.Core.Attributes;

namespace Cloudents.Infrastructure.Database
{

    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Api)]
    [ModuleRegistration(Core.Enum.System.Function)]
    //[ModuleRegistration(Core.Enum.System.Web)]
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactory>().AsSelf().As<IUnitOfWorkFactory>();
            builder.RegisterType<UnitOfWork>().AsSelf().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWorkAutofacFactory>().AsSelf().SingleInstance();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();
            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            base.Load(builder);
        }
    }
}