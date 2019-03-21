﻿using System.Reflection;
using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Persistence.Repositories;
using Cloudents.Query;
using Module = Autofac.Module;

namespace Cloudents.Persistence
{
 
    public class ModuleDb : Module
    {
      
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkFactorySpitball>()
                .SingleInstance();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenSession())
                .InstancePerLifetimeScope();

            builder.Register(c => c.Resolve<UnitOfWorkFactorySpitball>().OpenStatelessSession())
                .InstancePerLifetimeScope();


            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerDependency();
            builder.RegisterGeneric(typeof(NHibernateRepository<>))
                .AsImplementedInterfaces().InstancePerDependency();

            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(NHibernateRepository<>))
                .AsSelf()
                .AsImplementedInterfaces().InstancePerDependency();

            builder.RegisterType<QuerySession>().InstancePerDependency();

            base.Load(builder);
        }
    }
}