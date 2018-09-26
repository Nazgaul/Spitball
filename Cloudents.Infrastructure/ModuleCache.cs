using System;
using Autofac;
using CacheManager.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Cache;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [ModuleRegistration(Core.Enum.System.WorkerRole)]
    [ModuleRegistration(Core.Enum.System.Admin)]
    public class ModuleCache : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => CacheFactory.Build(settings =>
            {
                var key = c.Resolve<IConfigurationKeys>().Redis;
                settings
                    .WithMicrosoftMemoryCacheHandle("inProcessCache")
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromSeconds(10))
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

    [ModuleRegistration(Core.Enum.System.Function)]
    public class ModuleCacheFunction : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => CacheFactory.Build(settings =>
            {
                var key = c.Resolve<IConfigurationKeys>().Redis;
                settings
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