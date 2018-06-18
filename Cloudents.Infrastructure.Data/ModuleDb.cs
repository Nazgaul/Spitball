﻿using Autofac;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data.Repositories;
using JetBrains.Annotations;
using NHibernate;

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

            

            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();

            //builder.Register(c => new UnitOfWork(c.Resolve<ISession>()))
            //    /*.Keyed<IUnitOfWork>(Database.System)*/.InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();

            builder.RegisterType<QuestionSubjectRepository>().AsImplementedInterfaces();
            builder.RegisterType<QuestionRepository>().AsImplementedInterfaces();
            builder.RegisterType<UserRepository>().AsImplementedInterfaces();
            builder.RegisterType<CourseRepository>().AsImplementedInterfaces();
            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

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