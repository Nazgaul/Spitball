using Autofac;
using Cloudents.Core;
using Cloudents.Infrastructure.Framework.Database;

namespace Cloudents.Infrastructure.Framework
{
    public class ModuleDb : Module
    {
        private readonly string _sqlConnectionString;

        public ModuleDb(string sqlConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(_ => new UnitOfWorkFactory(_sqlConnectionString)).As<IUnitOfWorkFactory>().SingleInstance();
            builder.Register(c =>
            {
                var p = c.Resolve<IUnitOfWorkFactory>().OpenSession();
                return new UnitOfWork(p);
            }).InstancePerLifetimeScope().As<IUnitOfWork>();
            builder.RegisterGeneric(typeof(NHibernateRepository<>)).AsImplementedInterfaces();

            builder.RegisterModule<ModuleCore>();
            base.Load(builder);
        }
    }
}