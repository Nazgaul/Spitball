using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Persistence.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Mapping;
using NHibernate.Tool.hbm2ddl;
using System;
using System.Linq;
using System.Reflection;
using Autofac;
using ForeignKey = FluentNHibernate.Conventions.Helpers.ForeignKey;

namespace Cloudents.Persistence
{
    public class UnitOfWorkFactorySpitball : IStartable
    {
        private readonly PublishEventsListener _publisher;
        private readonly ISessionFactory _factory;

        public UnitOfWorkFactorySpitball(PublishEventsListener publisher,
            IInterceptor interceptor,
            IConfigurationKeys connectionString)
        {
            _publisher = publisher;
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.Db.Db)
                        .DefaultSchema("sb").Dialect<SbDialect>()
#if DEBUG
                        .ShowSql()
#endif
                ).ExposeConfiguration((x) => BuildSchema(x, interceptor, connectionString.Db.Integration));

            configuration.Mappings(m =>
            {
                m.FluentMappings.AddFromAssemblyOf<UserMap>()
                    .Conventions.Add(ForeignKey.EndsWith("Id"));

            });




            //TODO: Redis sometime fails. We need to gracefully fallback if it happens
            //configuration.Cache(c =>
            //{
            //    CoreDistributedCacheProvider.CacheFactory = new RedisFactory(connectionString.Db.Redis, "master");
            //    c.UseSecondLevelCache().RegionPrefix("nhibernate")
            //        .UseQueryCache().ProviderClass<CoreDistributedCacheProvider>();
            //});

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

        private void BuildSchema(Configuration config, IInterceptor interceptor,
            DbConnectionString.DataBaseIntegration needValidate)
        {
            SchemaMetadataUpdater.QuoteTableAndColumns(config, new SbDialect());
#if DEBUG
            if (interceptor != null)
            {
                config.SetInterceptor(interceptor);
            }
#endif
            // var eventPublisherListener = new PublishEventsListener(_publisher);
            config.SetListener(ListenerType.PostCommitDelete, _publisher);
            config.SetListener(ListenerType.Delete, new SoftDeleteEventListener());
            config.SetListener(ListenerType.PostCommitInsert, _publisher);
            config.SetListener(ListenerType.PostCommitUpdate, _publisher);
            config.SetListener(ListenerType.PostCollectionUpdate, _publisher);


            foreach (var t in Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(AbstractAuxiliaryDatabaseObject).IsAssignableFrom(t)))
            {
                config.AddAuxiliaryDatabaseObject(Activator.CreateInstance(t) as IAuxiliaryDatabaseObject);
            }


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
            switch (needValidate)
            {
                case DbConnectionString.DataBaseIntegration.None:
                    break;
                case DbConnectionString.DataBaseIntegration.Validate:
                    config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
                    break;
                case DbConnectionString.DataBaseIntegration.Update:
                    config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Update);
                    break;
            }
        }

        public void Start()
        {
            //Do nothing
        }
    }


}
