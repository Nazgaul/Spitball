using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace Cloudents.Infrastructure.Framework.Test
{
    public class InMemoryDatabaseTest : IDisposable
    {
        private ISessionFactory _sessionFactory;
        private ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var assemblyMapping = Assembly.Load("Cloudents.Infrastructure.Framework");
                    _sessionFactory = Fluently.Configure()
                        .Database(SQLiteConfiguration
                            .Standard
                            .InMemory()
                            .UsingFile("facultydata.db")
                        )
                        .Mappings(m => m.FluentMappings.AddFromAssembly(assemblyMapping))
                        .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                        .BuildSessionFactory();
                }
                return _sessionFactory;
            }
        }
        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }


        public void Dispose()
        {
            _sessionFactory?.Dispose();
        }
    }
}
