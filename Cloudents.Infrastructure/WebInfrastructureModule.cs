using System;
using System.Collections.Generic;
using Autofac;
using CacheManager.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cloudents.Infrastructure
{
    public class WebInfrastructureModule : InfrastructureModule
    {
        public WebInfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString,
            string storageConnectionString) :
            base(sqlConnectionString, searchServiceName, searchServiceKey, redisConnectionString, storageConnectionString)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SeoFlashcardRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Flashcard);
            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Item);
            //builder.RegisterType<TutorMeSearch>().As<ITutorProvider>();
            ConfigureCache(builder);
        }

        protected void ConfigureCache(ContainerBuilder builder)
        {
            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithMicrosoftMemoryCacheHandle().WithExpiration(ExpirationMode.Sliding, TimeSpan.FromHours(1));
                if (!string.IsNullOrEmpty(RedisConnectionString))
                {
                    settings.WithJsonSerializer();
                    settings.WithRedisConfiguration("redis", RedisConnectionString)
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


    public class MobileAppInfrastructureModule : InfrastructureModule
    {
        public MobileAppInfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString,
            string storageConnectionString) :
            base(sqlConnectionString, searchServiceName, searchServiceKey, redisConnectionString, storageConnectionString)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(_ =>
            {
                var x = new DbContextOptionsBuilder<AppDbContext>();
                x.UseSqlServer(SqlConnectionString);
                return new AppDbContext(x.Options);
            });
            ConfigureCache(builder);
            //builder.RegisterType<TutorMeSearch>().As<ITutorProvider>();

        }


        protected void ConfigureCache(ContainerBuilder builder)
        {
            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                if (!string.IsNullOrEmpty(RedisConnectionString))
                {
                    settings.WithJsonSerializer();
                    settings.WithRedisConfiguration("redis", RedisConnectionString)
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
