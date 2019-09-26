﻿using Cloudents.Core.Interfaces;
using Cloudents.Persistence.Maps;
using FluentNHibernate.Cfg;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using System.Reflection;

namespace Cloudents.Persistence
{
    public class UnitOfWorkFactorySpitball
    {
        private readonly PublishEventsListener _publisher;
        private readonly ISessionFactory _factory;

        public UnitOfWorkFactorySpitball(PublishEventsListener publisher, IConfigurationKeys connectionString)
        {
            _publisher = publisher;
//            var configuration = Fluently.Configure()
//                .Database(
//                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.Db.Db)
//                        .DefaultSchema("sb").Dialect<SbDialect>()

//#if DEBUG
//                        .ShowSql()
//#endif
//                ).ExposeConfiguration(BuildSchema);

//            configuration.Mappings(m =>
//            {
//                m.FluentMappings.AddFromAssemblyOf<UserMap>()
//                    .Conventions.Add(ForeignKey.EndsWith("Id"));
//            });

            Configuration configuration = new Configuration()
             .DataBaseIntegration(db =>
             {
                 db.ConnectionString = connectionString.Db.Db;
                 db.Dialect<SbDialect>();
             }).SetProperty("default_schema", "sb");
            /* Add the mapping we defined: */
            var mapper = new ModelMapper();
            mapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
            
            HbmMapping mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            configuration.AddMapping(mapping);




            //TODO: Redis sometime fails. We need to gracefully fallback if it happens
            //configuration.Cache(c =>
            //{
            //    CoreDistributedCacheProvider.CacheFactory = new RedisFactory(connectionString.Db.Redis, "master");
            //    c.UseSecondLevelCache().RegionPrefix("nhibernate")
            //        .UseQueryCache().ProviderClass<CoreDistributedCacheProvider>();
            //});
            configuration.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
            _factory = configuration.BuildSessionFactory();

        }

        public ISession OpenSession()
        {
            var session = _factory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }

        //public ISessionFactory GetFactory()
        //{
        //    return _factory;
        //}

        public IStatelessSession OpenStatelessSession()
        {
            return _factory.OpenStatelessSession();
        }

        private void BuildSchema(Configuration config)
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(config, new SbDialect());
#if DEBUG
            config.SetInterceptor(new LoggingInterceptor());
#endif
           // var eventPublisherListener = new PublishEventsListener(_publisher);
            config.SetListener(ListenerType.PostCommitDelete, _publisher);
            config.SetListener(ListenerType.Delete, new SoftDeleteEventListener());
            config.SetListener(ListenerType.PostCommitInsert, _publisher);
            config.SetListener(ListenerType.PostCommitUpdate, _publisher);
            config.SetListener(ListenerType.PostCollectionUpdate, _publisher);


            //var enversConf = new NHibernate.Envers.Configuration.Fluent.FluentConfiguration();
            //enversConf.Audit<Document>()
            //    .Exclude(x => x.Transactions)
            //    .ExcludeRelationData(x => x.University)
            //    .Exclude(x => x.Votes)
            //    .Exclude(x => x.Course)
            //    .Exclude(x => x.User)
            //    .Exclude(x => x.Status.FlaggedUser)
            //    .ExcludeRelationData(x => x.Tags);


            //config.IntegrateWithEnvers(enversConf);
            config.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();

            //config.SessionFactory().Caching.WithDefaultExpiration(TimeConst.Day);
            //config.Properties.Add("cache.default_expiration",$"{TimeConst.Day}");
            //config.Properties.Add("cache.use_sliding_expiration",bool.TrueString.ToLowerInvariant());
            //config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Update);
        }
    }
}
