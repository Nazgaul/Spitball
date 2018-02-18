using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Cloudents.Infrastructure.Framework.Test
{
    public class InMemoryDatabaseTest
    {
        private ISessionFactory SessionFactory;
        protected ISession session;

        public InMemoryDatabaseTest()
        {
            var assemblyMapping = Assembly.Load("Cloudents.Infrastructure.Framework");
            var configuration = Fluently.Configure()
                .Database(() => SQLiteConfiguration.Standard.InMemory().ShowSql())
                .Mappings(x => x.FluentMappings.AddFromAssembly(assemblyMapping))
                .ExposeConfiguration(
                        cfg =>
                        {
                            // cfg.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Create);
                        });
            SessionFactory = configuration.BuildSessionFactory();
            session = SessionFactory.OpenSession();
            new SchemaExport(configuration.BuildConfiguration()).Execute(true, true, false, session.Connection, Console.Out);
        }

    }
}
