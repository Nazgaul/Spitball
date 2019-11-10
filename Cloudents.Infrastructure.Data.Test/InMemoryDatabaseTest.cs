using Cloudents.Persistence.Maps;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Testing;
using NHibernate;
using System;

namespace Cloudents.Infrastructure.Data.Test
{
    public class InMemoryDatabaseTest : IDisposable
    {
        protected readonly ISession Session;
        // protected readonly ISessionFactory sessionFactory;

        public InMemoryDatabaseTest()
        {

            var configuration = Fluently.Configure()
                .Database(() => SQLiteConfiguration.Standard.InMemory())

                .Mappings(x => x.FluentMappings.AddFromAssembly(typeof(UserMap).Assembly))
            .ExposeConfiguration(x =>
                {
                    x.SetProperty(NHibernate.Cfg.Environment.ReleaseConnections, "on_close");
                });
            var sessionSource = new SingleConnectionSessionSourceForSQLiteInMemoryTesting(configuration);
            sessionSource.BuildSchema();
            Session = sessionSource.CreateSession();


            //var assemblyMapping = Assembly.Load("Cloudents.Persistence");
            //var configuration = Fluently.Configure()
            //    .Database(() => SQLiteConfiguration.Standard.InMemory().ShowSql().DefaultSchema("sb"))

            //    .Mappings(x => x.FluentMappings.AddFromAssembly(assemblyMapping))
            //    .ExposeConfiguration(
            //            cfg =>
            //            {
            //                 //cfg.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
            //            });
            //var sessionFactory = configuration.BuildSessionFactory();
            //Session = sessionFactory.OpenSession();
            //new SchemaExport(configuration.BuildConfiguration()).Execute(true, true, false, Session.Connection, Console.Out);
        }

        public void Dispose()
        {
            Session?.Dispose();
            //sessionFactory?.Dispose();
        }
    }

    //public class InMemoryDatabaseTest : IDisposable
    //{
    //    private static Configuration Configuration;
    //    private static ISessionFactory SessionFactory;
    //    protected ISession session;

    //    public InMemoryDatabaseTest(Assembly assemblyContainingMapping)
    //    {
    //        if (Configuration == null)
    //        {
    //            Configuration = new Configuration()
    //                .SetProperty(Environment.ReleaseConnections, "on_close")
    //                .SetProperty(Environment.Dialect, typeof(SQLiteDialect).AssemblyQualifiedName)
    //                .SetProperty(Environment.ConnectionDriver, typeof(SQLite20Driver).AssemblyQualifiedName)
    //                .SetProperty(Environment.ConnectionString, "data source=:memory:")
    //                .SetProperty(Environment.ProxyFactoryFactoryClass, typeof(ProxyFactoryFactory).AssemblyQualifiedName)
    //                .AddAssembly(assemblyContainingMapping);

    //            SessionFactory = Configuration.BuildSessionFactory();
    //        }

    //        session = SessionFactory.OpenSession();

    //        new SchemaExport(Configuration).Execute(true, true, false, true, session.Connection, Console.Out);
    //    }

    //    public void Dispose()
    //    {
    //        session.Dispose();
    //    }
    //}
}
