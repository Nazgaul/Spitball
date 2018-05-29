using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Infrastructure.Database.Maps;
using FluentNHibernate.Cfg;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Cfg;

namespace Cloudents.Infrastructure.Database
{
    [UsedImplicitly]
    public class UnitOfWorkFactory //: IUnitOfWorkFactory
    {
        private readonly ISessionFactory _factory;

        public delegate UnitOfWorkFactory Factory(Core.Enum.Database db);

        private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                from z in x.GetInterfaces()
                let y = x.BaseType
                where
                    y?.IsGenericType == true
                              && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())
                    || z.IsGenericType
                    && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition())
                   select x;
        }

        public UnitOfWorkFactory(Core.Enum.Database db, DbConnectionStringProvider connectionString)
        {
            //TODO: CREATE SCHEMA sb
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.GetConnectionString(db))
                        .DefaultSchema("sb")
#if DEBUG
                        .ShowSql
#endif

                    )
                .ExposeConfiguration(BuildSchema);
            if (db == Core.Enum.Database.System)
            {
                configuration.Mappings(m =>
                {
                    var types = GetAllTypesImplementingOpenGenericType(typeof(SpitballClassMap<>),
                        Assembly.GetExecutingAssembly());
                    foreach (var type in types)
                    {
                        m.FluentMappings.Add(type);
                    }
                });
            }
            else
            {
                configuration.Mappings(m =>
                {
                    m.FluentMappings.Add<MailGunStudentMap>();
                });
            }
            _factory = configuration.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return _factory.OpenSession();
        }

        private static void BuildSchema(Configuration config)
        {
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Update);
        }
    }
}
