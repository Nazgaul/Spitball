using System;
using Autofac;
using CacheManager.Core;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    public class ModuleCache : Module
    {
        private readonly string _redisConnectionString;

        public ModuleCache(string redisConnectionString)
        {
            _redisConnectionString = redisConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => CacheFactory.Build(settings =>
            {
                //settings.WithMicrosoftMemoryCacheHandle();//.WithExpiration(ExpirationMode.Sliding, TimeSpan.FromHours(1));
                //if (!string.IsNullOrEmpty(_redisConnectionString))
                //{
                //    settings.WithJsonSerializer();
                //    settings.WithRedisConfiguration("redis", _redisConnectionString)
                //        .WithRedisBackplane("redis")
                //        .WithRedisCacheHandle("redis");
                //}

                settings
                    .WithMicrosoftMemoryCacheHandle("inProcessCache")
                    .And
                    .WithRedisConfiguration("redis", _redisConnectionString)
                    .WithJsonSerializer()
                    .WithMaxRetries(1000)
                    .WithRetryTimeout(100)
                    .WithRedisBackplane("redis")
                    .WithRedisCacheHandle("redis", true);
            })).AsSelf().SingleInstance().AsImplementedInterfaces();
            //var cache = CacheFactory.Build(settings =>
            //{
            //    settings.WithMicrosoftMemoryCacheHandle().WithExpiration(ExpirationMode.Sliding, TimeSpan.FromHours(1));
            //    if (!string.IsNullOrEmpty(_redisConnectionString))
            //    {
            //        settings.WithJsonSerializer();
            //        settings.WithRedisConfiguration("redis", _redisConnectionString)
            //            .WithRedisBackplane("redis").WithRedisCacheHandle("redis");
            //    }
            //});
            //builder.RegisterGeneric(typeof(BaseCacheManager<>))
            //    .WithParameters(new[]
            //    {
            //        new TypedParameter(typeof(ICacheManagerConfiguration), cacheConfig)
            //    })
            //    .As(typeof(ICacheManager<>))
            //    .SingleInstance();

            builder.RegisterType<CacheProvider>().AsImplementedInterfaces();
        }
    }
}