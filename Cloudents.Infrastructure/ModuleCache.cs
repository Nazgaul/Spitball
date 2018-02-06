using Autofac;
using CacheManager.Core;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    public class ModuleCache : Module
    {
        //private readonly string _redisConnectionString;

        //public ModuleCache(string redisConnectionString)
        //{
        //    _redisConnectionString = redisConnectionString;
        //}

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => CacheFactory.Build(settings =>
            {
                var key = c.Resolve<IConfigurationKeys>().Redis;
                settings
                    .WithMicrosoftMemoryCacheHandle("inProcessCache")
                    .And
                    .WithRedisConfiguration("redis", key)
                    .WithJsonSerializer()
                    .WithMaxRetries(1000)
                    .WithRetryTimeout(100)
                    .WithRedisBackplane("redis")
                    .WithRedisCacheHandle("redis");
            })).AsSelf().SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<CacheProvider>().AsImplementedInterfaces();
        }
    }
}