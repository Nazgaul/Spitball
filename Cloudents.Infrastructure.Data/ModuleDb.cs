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

            
            builder.RegisterType<UnitOfWorkFactorySpitball>().SingleInstance();//.Keyed<IUnitOfWorkFactory>(Database.System).SingleInstance();
            builder.RegisterType<UnitOfWorkFactoryMailGun>().SingleInstance();//.Keyed<IUnitOfWorkFactory>(Database.MailGun).SingleInstance();
            //builder.RegisterType<UnitOfWork>().AsSelf().As<IUnitOfWork>().InstancePerLifetimeScope();


            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactorySpitball>()))
                .Keyed<IUnitOfWork>(Database.System).InstancePerLifetimeScope();
            builder.Register(c => new UnitOfWork(c.Resolve<UnitOfWorkFactoryMailGun>()))
                .Keyed<IUnitOfWork>(Database.MailGun).InstancePerLifetimeScope();
            //builder.RegisterType<UnitOfWork>().Keyed<IUnitOfWork>(Database.System).InstancePerLifetimeScope();
            //builder.RegisterType<UnitOfWork>().Keyed<IUnitOfWork>(Database.MailGun).InstancePerLifetimeScope();
            //builder.RegisterType<UnitOfWorkAutofacFactory>().AsSelf().SingleInstance();
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