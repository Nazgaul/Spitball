using System.Reflection;
using Cloudents.Core;
using Cloudents.Infrastructure.Framework.Database.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;

namespace Cloudents.Infrastructure.Framework.Database
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly ISessionFactory _factory;

        public delegate UnitOfWorkFactory Factory(Core.Enum.Database db);

        public UnitOfWorkFactory(Core.Enum.Database db, DbConnectionStringProvider connectionString)
        {

            var configuration = Fluently.Configure()
                .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.GetConnectionString(db)))

                .ExposeConfiguration(BuildSchema);
            if (db == Core.Enum.Database.System)
            {
                configuration.Mappings(m =>
                {
                    m.FluentMappings.Add<CourseMap>();
                    m.FluentMappings.Add<UniversityMap>();
                    m.FluentMappings.Add<UrlStatsMap>();
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

        //public UnitOfWorkFactory(DbConnectionStringProvider connectionString)
        //{
        //    var configuration = Fluently.Configure()
        //        .Database(FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
        //        .Mappings(m =>
        //        {
        //            m.FluentMappings.Add<CourseMap>();
        //            m.FluentMappings.Add<UniversityMap>();
        //            m.FluentMappings.Add<UrlStatsMap>();
        //        })
        //        .ExposeConfiguration(BuildSchema);
        //    _factory = configuration.BuildSessionFactory();
        //}

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
