﻿using Cloudents.Core;
using Cloudents.Infrastructure.Database.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;

namespace Cloudents.Infrastructure.Database
{
    public class UnitOfWorkFactoryMailGun 
    {
        private readonly ISessionFactory _factory;

        public UnitOfWorkFactoryMailGun(DbConnectionStringProvider connectionString)
        {
            //TODO: CREATE SCHEMA sb
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.GetConnectionString(Core.Enum.Database.MailGun))
                    //#if DEBUG
                    //                        .ShowSql
                    //#endif

                    )
                .ExposeConfiguration(BuildSchema);

                configuration.Mappings(m =>
                {
                    m.FluentMappings.Add<MailGunStudentMap>();
                });
            _factory = configuration.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return _factory.OpenSession();
        }

        private static void BuildSchema(Configuration config)
        {
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
        }
    }
}