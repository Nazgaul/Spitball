using Autofac;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Data.Repositories;
using JetBrains.Annotations;

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

            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactorySpitball>()))
                .Keyed<IUnitOfWork>(Database.System).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();

            builder.RegisterType<QuestionSubjectRepository>().AsImplementedInterfaces();
            builder.RegisterType<QuestionRepository>().AsImplementedInterfaces();
            builder.RegisterType<UserRepository>().AsImplementedInterfaces();
            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            base.Load(builder);
        }
    }

    [ModuleRegistration(Core.Enum.System.Function)]
    [UsedImplicitly]
    public class ModuleMailGun : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactoryMailGun>().SingleInstance();
            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactoryMailGun>()))
                .Keyed<IUnitOfWork>(Database.MailGun).InstancePerLifetimeScope();
        }

    }
}