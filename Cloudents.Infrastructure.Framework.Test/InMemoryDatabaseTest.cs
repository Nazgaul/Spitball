using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Testing;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Tool.hbm2ddl;
using Environment = NHibernate.Cfg.Environment;

namespace Cloudents.Infrastructure.Framework.Test
{
    public class InMemoryDatabaseTest 
    {
        private ISession _sessionFactory;
        public ISession SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    var assemblyMapping = Assembly.Load("Cloudents.Infrastructure.Framework");
                    var configuration = Fluently.Configure()
                        .Database(() => SQLiteConfiguration.Standard.InMemory().ShowSql())
                        .Mappings(x => x.FluentMappings.AddFromAssembly(assemblyMapping))
                        ;
                    var sessionSource = new SingleConnectionSessionSourceForSQLiteInMemoryTesting(configuration);

                    _sessionFactory = sessionSource.CreateSession();
                    //_sessionFactory = Fluently.Configure()
                    //    .Database(SQLiteConfiguration
                    //        .Standard
                    //        .InMemory()
                    //        .UsingFile("facultydata.db")
                    //    )
                    //    .Mappings(m => m.FluentMappings.AddFromAssembly(assemblyMapping))
                    //    .ExposeConfiguration(cfg => new SchemaExport(cfg).Create(true, true))
                    //    .BuildSessionFactory();
                }

                //var x = new FluentNHibernate.Testing.SingleConnectionSessionSourceForSQLiteInMemoryTesting()

                return _sessionFactory;
            }
        }
       
    }
}
