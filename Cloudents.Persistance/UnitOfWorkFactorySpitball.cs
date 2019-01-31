using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Persistance.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Caches.CoreDistributedCache;
using NHibernate.Caches.CoreDistributedCache.Redis;
using NHibernate.Cfg;
using NHibernate.Event;
//using NHibernate.Caches.CoreDistributedCache;
//using NHibernate.Caches.CoreDistributedCache.Redis;

namespace Cloudents.Persistance
{
    public class UnitOfWorkFactorySpitball
    {
        private readonly IEventPublisher _publisher;
        private readonly ISessionFactory _factory;


        //private static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        //{
        //    return from x in assembly.GetTypes()
        //        from z in x.GetInterfaces()
        //        let y = x.BaseType
        //        where
        //            (y?.IsGenericType == true
        //             && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition()))
        //            || (z.IsGenericType
        //                && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
        //        select x;
        //}

        public UnitOfWorkFactorySpitball(IEventPublisher publisher, IConfigurationKeys connectionString)
        {
            _publisher = publisher;
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.Db.Db)
                        .DefaultSchema("sb").Dialect<SbDialect>()

#if DEBUG
                        .ShowSql()
#endif
                ).ExposeConfiguration(BuildSchema);

            configuration.Mappings(m =>
            {
                //var types = GetAllTypesImplementingOpenGenericType(typeof(SpitballClassMap<>),
                //    Assembly.GetExecutingAssembly());
                m.FluentMappings.AddFromAssemblyOf<UserMap>();
                //foreach (var type in types)
                //{
                //    m.FluentMappings.Add(type);
                //}
                //m.FluentMappings.Add<DomainTimeStampMap>();
                //m.FluentMappings.Add<RowDetailMap>();
            });



            //TODO: Azure function as usual making live harder
            //Could not load file or assembly 'Microsoft.Extensions.Options, Version=2.0.0.0, Culture=neutral, PublicKeyToken=adb9793829ddae60' or one of its dependencies. The system cannot find the file specified.
            configuration.Cache(c =>
            {
                CoreDistributedCacheProvider.CacheFactory = new RedisFactory(connectionString.Db.Redis, "master");
                c.UseSecondLevelCache().RegionPrefix("nhibernate")
                    .UseQueryCache().ProviderClass<CoreDistributedCacheProvider>();
            });

            _factory = configuration.BuildSessionFactory();

        }

        public ISession OpenSession()
        {
            var session = _factory.OpenSession();
            session.FlushMode = FlushMode.Commit;
            return session;
        }

        public ISessionFactory GetFactory()
        {
            return _factory;
        }

        public IStatelessSession OpenStatelessSession()
        {
            return _factory.OpenStatelessSession();
        }

        private void BuildSchema(Configuration config)
        {
#if DEBUG
            config.SetInterceptor(new LoggingInterceptor());
#endif
            var eventPublisherListener = new PublishEventsListener(_publisher);
            config.SetListener(ListenerType.PostCommitDelete, eventPublisherListener);
            config.SetListener(ListenerType.Delete, new SoftDeleteEventListener());
            config.SetListener(ListenerType.PostInsert, eventPublisherListener);
            config.SetListener(ListenerType.PostUpdate, eventPublisherListener);
            config.SetListener(ListenerType.PostCollectionUpdate, eventPublisherListener);


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
            //config.LinqToHqlGeneratorsRegistry<MyLinqToHqlGeneratorsRegistry>();

            //config.SessionFactory().Caching.WithDefaultExpiration(TimeConst.Day);
            //config.Properties.Add("cache.default_expiration",$"{TimeConst.Day}");
            //config.Properties.Add("cache.use_sliding_expiration",bool.TrueString.ToLowerInvariant());
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Update);
        }
    }
}
