using Autofac;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Data.Repositories;
using JetBrains.Annotations;

namespace Cloudents.Infrastructure.Data
{
    [ModuleRegistration(Core.Enum.System.Console)]
    //[ModuleRegistration(Core.Enum.System.Function)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.FunctionV1)]
    [UsedImplicitly]
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<UnitOfWorkFactory>().AsSelf();//.As<IUnitOfWorkFactory>();

            
            builder.RegisterType<UnitOfWorkFactorySpitball>().SingleInstance();
            builder.RegisterType<UnitOfWorkFactoryMailGun>().SingleInstance();

            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactorySpitball>()))
                .Keyed<IUnitOfWork>(Database.System).InstancePerLifetimeScope();
            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactoryMailGun>()))
                .Keyed<IUnitOfWork>(Database.MailGun).InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();

            builder.RegisterType<QuestionSubjectRepository>().AsImplementedInterfaces();
            builder.RegisterType<QuestionRepository>().AsImplementedInterfaces();
            builder.RegisterType<UserRepository>().AsImplementedInterfaces();
            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            base.Load(builder);
        }
    }
}