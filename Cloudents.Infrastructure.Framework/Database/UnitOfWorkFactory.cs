using System.Reflection;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISessionFactory _factory;

        public UnitOfWorkFactory(string connectionString)
        {
            var configuration = Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
                .ExposeConfiguration(BuildSchema);
            
            _factory = configuration.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return _factory.OpenSession();
        }

        private static void BuildSchema(Configuration config)
        {
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Update);
            config.SetListener(ListenerType.Delete, new DeleteEventListener());

            config.SetListener(ListenerType.PreInsert, new InsertUpdateEventListener());
            config.SetListener(ListenerType.PreUpdate, new InsertUpdateEventListener());
            // delete the existing db on each run

            //config.SetProperty(Environment., "true");
            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            //new SchemaValidator(config);
            //new SchemaExport(config)
            //    .Create(false, true);
        }

       
    }
}
