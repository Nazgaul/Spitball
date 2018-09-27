﻿using System;
using System.Reflection;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Cloudents.Infrastructure.Data.Test
{
    public class InMemoryDatabaseTest
    {
        protected static readonly ISession Session;

        static InMemoryDatabaseTest()
        {
            var assemblyMapping = Assembly.Load("Cloudents.Infrastructure");
            var configuration = Fluently.Configure()
                .Database(() => SQLiteConfiguration.Standard.InMemory().ShowSql().DefaultSchema("sb"))
                
                .Mappings(x => x.FluentMappings.AddFromAssembly(assemblyMapping))
                .ExposeConfiguration(
                        cfg =>
                        {
                             //cfg.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
                        });
            var sessionFactory = configuration.BuildSessionFactory();
            Session = sessionFactory.OpenSession();
            new SchemaExport(configuration.BuildConfiguration()).Execute(true, true, false, Session.Connection, Console.Out);
        }
    }
}
