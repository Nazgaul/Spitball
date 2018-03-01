﻿using Autofac;
using Autofac.Core;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Framework.Database;
using NHibernate;

namespace Cloudents.Infrastructure.Framework
{
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register(c =>
            //{
            //    var key = c.Resolve<IConfigurationKeys>().Db;
            //    return new UnitOfWorkFactory(key);
            //}).As<IUnitOfWorkFactory>().SingleInstance();

            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>().SingleInstance();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            //builder.Register((c,t) =>
            //{
            //    var p = c.Resolve<IUnitOfWorkFactory>().OpenSession();
            //    return new UnitOfWork(p);
            //}).InstancePerLifetimeScope().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces();

            builder.RegisterType<DbConnectionStringProvider>().AsSelf();

            //builder.RegisterModule<ModuleCore>();
            base.Load(builder);
        }
    }
}