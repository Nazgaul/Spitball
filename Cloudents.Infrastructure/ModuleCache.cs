using System;
using Autofac;
using CacheManager.Core;

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

            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithMicrosoftMemoryCacheHandle().WithExpiration(ExpirationMode.Sliding, TimeSpan.FromHours(1));
                if (!string.IsNullOrEmpty(_redisConnectionString))
                {
                    settings.WithJsonSerializer();
                    settings.WithRedisConfiguration("redis", _redisConnectionString)
                        .WithRedisBackplane("redis").WithRedisCacheHandle("redis");
                }
            });
            builder.RegisterGeneric(typeof(BaseCacheManager<>))
                .WithParameters(new[]
                {
                    new TypedParameter(typeof(ICacheManagerConfiguration), cacheConfig)
                })
                .As(typeof(ICacheManager<>))
                .SingleInstance();

        }
    }
}