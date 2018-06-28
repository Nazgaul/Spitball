using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.Enum;
using Cloudents.Infrastructure.Data.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;

namespace Cloudents.Infrastructure.Data
{
    public class UnitOfWorkFactorySpitball : IUnitOfWorkFactory
    {
        private readonly ISessionFactory _factory;

        private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                       (y?.IsGenericType == true
                                 && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition()))
                       || (z.IsGenericType
                       && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }

        public UnitOfWorkFactorySpitball(DbConnectionStringProvider connectionString)
        {
            //TODO: CREATE SCHEMA sb
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.GetConnectionString(Database.System))
                        .DefaultSchema("sb").Dialect<SbDialect>()
#if DEBUG
                        .ShowSql()
#endif
               ).ExposeConfiguration(BuildSchema);

            configuration.Mappings(m =>
            {
                var types = GetAllTypesImplementingOpenGenericType(typeof(SpitballClassMap<>),
                    Assembly.GetExecutingAssembly());
                foreach (var type in types)
                {
                    m.FluentMappings.Add(type);
                }
            });
            _factory = configuration.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            var session = _factory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }

        public IStatelessSession OpenStatelessSession()
        {
            return _factory.OpenStatelessSession();
        }

        private static void BuildSchema(Configuration config)
        {
#if DEBUG
            config.SetInterceptor(new LoggingInterceptor());
#endif
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
        }
    }
}