using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cloudents.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Data.Maps;
using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Caches.CoreDistributedCache;
using NHibernate.Caches.CoreDistributedCache.Redis;
using NHibernate.Cfg;

namespace Cloudents.Infrastructure.Data
{
    public class UnitOfWorkFactorySpitball
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

        public UnitOfWorkFactorySpitball(IConfigurationKeys connectionString)
        {
            var configuration = Fluently.Configure()
                .Database(
                    FluentNHibernate.Cfg.Db.MsSqlConfiguration.MsSql2012.ConnectionString(connectionString.Db)
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
            configuration.Cache(c =>
            {
                CoreDistributedCacheProvider.CacheFactory = new RedisFactory(connectionString.Redis, "master");
                c.UseSecondLevelCache().RegionPrefix("nhibernate")
                    .UseQueryCache().ProviderClass<CoreDistributedCacheProvider>();
            });

            _factory = configuration.BuildSessionFactory();

            // _factory.Statistics.IsStatisticsEnabled = true;
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
            config.SessionFactory().Caching.WithDefaultExpiration(TimeConst.Day);
            //config.Properties.Add("cache.default_expiration",$"{TimeConst.Day}");
            //config.Properties.Add("cache.use_sliding_expiration",bool.TrueString.ToLowerInvariant());
            config.DataBaseIntegration(dbi => dbi.SchemaAction = SchemaAutoAction.Validate);
        }
    }
}
  