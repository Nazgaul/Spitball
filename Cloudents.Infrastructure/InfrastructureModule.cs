using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using CacheManager.Core;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Storage;
using Microsoft.Azure.Search;
using Microsoft.Cognitive.LUIS;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly string _sqlConnectionString;
        private readonly string _searchServiceName;
        private readonly string _searchServiceKey;
        private readonly string _redisConnectionString;

        private readonly string _storageConnectionString;
       // private readonly Environment _environment;

        public InfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString, string storageConnectionString
            //Environment environment
            )
        {
            _sqlConnectionString = sqlConnectionString;
            _searchServiceName = searchServiceName;
            _searchServiceKey = searchServiceKey;
            _redisConnectionString = redisConnectionString;
            _storageConnectionString = storageConnectionString;
            // _environment = environment;
        }

        protected override void Load(ContainerBuilder builder)
        {
            //builder.Register(c => new CacheResultInterceptor(c.Resolve<ICacheProvider>())); //<CacheResultInterceptor>();
            builder.RegisterType<CacheResultInterceptor>();

            builder.RegisterType<LuisAI>().As<IAI>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<EngineProcess>().As<IEngineProcess>();
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();

            builder.Register(c => new DapperRepository(_sqlConnectionString));
            builder.Register(c => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40")).AsImplementedInterfaces();
            //builder.Register(c => new DocumentDbInitializer().GetClient("https://zboxnew.documents.azure.com:443/",
            //        "y2v1XQ6WIg81Soasz5YBA7R8fAp52XhJJufNmHy1t7y3YQzpBqbgRnlRPlatGhyGegKdsLq0qFChzOkyQVYdLQ=="))
            //    .As<IReliableReadWriteDocumentClient>().SingleInstance();

            //builder.RegisterGeneric(typeof(DocumentDbRepository<>))
            //    .AsImplementedInterfaces().SingleInstance()
            //    .WithParameter("databaseId", "Zbox")
            //    .WithParameter(new ResolvedParameter((pi, ctx) => pi.Name == "client",
            //    (pi, ctx) => ctx.Resolve<IReliableReadWriteDocumentClient>()
            //    ));

            builder.Register(c =>
                new SearchServiceClient(_searchServiceName, new SearchCredentials(_searchServiceKey)))
                .SingleInstance().AsSelf().As<ISearchServiceClient>();

            builder.RegisterType<CacheProvider>().AsImplementedInterfaces();
            builder.RegisterType<CseSearch>().As<ICseSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentCseSearch>().As<IDocumentCseSearch>();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>();
            builder.RegisterType<CourseSearch>().As<ICourseSearch>();
            builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>();

            builder.RegisterType<TitleSearch>().As<ITitleSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<VideoSearch>().As<IVideoSearch>();
            builder.RegisterType<JobSearch>().As<IJobSearch>();
            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<RestClient>().As<IRestClient>();
            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>();
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));

            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Item);


            builder.RegisterGeneric(typeof(EfRepository<>)).AsImplementedInterfaces();


            builder.Register(c => new CloudStorageProvider(_storageConnectionString)).SingleInstance();
            builder.RegisterType<BlobProvider>().AsImplementedInterfaces();

            ConfigureCache(builder);
            var config = MapperConfiguration();
            builder.Register(c => config.CreateMapper()).SingleInstance();
        }

        private void ConfigureCache(ContainerBuilder builder)
        {
            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithMicrosoftMemoryCacheHandle().WithExpiration(ExpirationMode.Sliding,TimeSpan.FromHours(1));
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

        private static MapperConfiguration MapperConfiguration()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        }
    }


}

