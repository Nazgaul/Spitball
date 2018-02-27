using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Framework.Database;

namespace Cloudents.Infrastructure.Framework
{
    public class ModuleDb : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Db;
                return new UnitOfWorkFactory(key);
            }).As<IUnitOfWorkFactory>().SingleInstance();
            builder.Register(c =>
            {
                var p = c.Resolve<IUnitOfWorkFactory>().OpenSession();
                return new UnitOfWork(p);
            }).InstancePerLifetimeScope().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(NHibernateRepository<>)).AsImplementedInterfaces();

            //builder.RegisterModule<ModuleCore>();
            base.Load(builder);
        }
    }
}