using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using CacheManager.Core;
using Castle.DynamicProxy;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Models;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Converters;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Newtonsoft.Json.Linq;
using Microsoft.Cognitive.LUIS;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly string m_SqlConnectionString;
        private readonly string m_SearchServiceName;
        private readonly string m_SearchServiceKey;
        private readonly string m_RedisConnectionString;
        private readonly Environment m_Environment;

        public InfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString, Environment environment)
        {
            m_SqlConnectionString = sqlConnectionString;
            m_SearchServiceName = searchServiceName;
            m_SearchServiceKey = searchServiceKey;
            m_RedisConnectionString = redisConnectionString;
            m_Environment = environment;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new CacheResultInterceptor(c.Resolve<ICacheProvider>())); //<CacheResultInterceptor>();
            builder.RegisterType<LuisAI>().As<IAI>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();

            builder.Register(c => new DapperRepository(m_SqlConnectionString));
            builder.Register(c => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40"));
            builder.Register(c =>
                new SearchServiceClient(m_SearchServiceName, new SearchCredentials(m_SearchServiceKey)))
                .SingleInstance().AsSelf().As<ISearchServiceClient>();

            builder.RegisterType(typeof(CacheProvider)).As(typeof(ICacheProvider));
            builder.RegisterType<CseSearch>().As<ICseSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>();
            builder.RegisterType<CourseSearch>().As<ICourseSearch>();
            builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>();
            if (m_Environment == Environment.Web)
            {
                builder.RegisterType<TutorMeSearch>().As<ITutorProvider>();
            }
            builder.RegisterType<TitleSearch>().As<ITitleSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<VideoSearch>().As<IVideoSearch>();
            builder.RegisterType<JobSearch>().As<IJobSearch>();
            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<RestClient>().As<IRestClient>();
            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();

            builder.RegisterType<UniversitySynonymRepository>().As<IReadRepositorySingle<UniversitySynonymDto, long>>();

            ConfigureCache(builder);
            var config = MapperConfiguration();
            builder.Register(c => config.CreateMapper()).SingleInstance();
        }

        private void ConfigureCache(ContainerBuilder builder)
        {
            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings
                    .WithSystemRuntimeCacheHandle("inProcess")
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10));
                if (!string.IsNullOrEmpty(m_RedisConnectionString))
                {
                    settings.WithJsonSerializer();

                    settings.WithRedisConfiguration("redis", m_RedisConnectionString)

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
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Search.Entities.Tutor, TutorDto>()
                    .ForMember(d => d.Location, o => o.ResolveUsing(p => new GeoPoint
                    {
                        Latitude = p.Location.Latitude,
                        Longitude = p.Location.Longitude
                    }))
                    .ForMember(d => d.TermCount, o => o.ResolveUsing((t, ts, i, c) =>
                    {
                        var temp = $"{t.City} {t.State} {string.Join(" ", t.Subjects)} {string.Join(" ", t.Extra)}";
                        return temp.Split(new[] { c.Items["term"].ToString() },
                            StringSplitOptions.RemoveEmptyEntries).Length;
                    }));

                cfg.CreateMap<DocumentSearchResult<Search.Entities.Job>, ResultWithFacetDto<JobDto>>()
                    .ConvertUsing<JobResultConverter>();


                cfg.CreateMap<Search.Entities.Job, JobDto>();
                cfg.CreateMap<Search.Entities.Course, CourseDto>();
                cfg.CreateMap<Search.Entities.University, UniversityDto>();


                cfg.CreateMap<JObject, IEnumerable<BookSearchDto>>().ConvertUsing((jo, bookSearch, c) => jo["response"]["page"]["books"]?["book"]?.Select(json => c.Mapper.Map<JToken, BookSearchDto>(json)));
                cfg.CreateMap<JToken, BookSearchDto>().ConvertUsing((jo) => new BookSearchDto
                {
                    Image = jo["image"]?["image"].Value<string>(),
                    Author = jo["author"].Value<string>(),
                    Binding = jo["binding"].Value<string>(),
                    Edition = jo["edition"].Value<string>(),
                    Isbn10 = jo["isbn10"].Value<string>(),
                    Isbn13 = jo["isbn13"].Value<string>(),
                    Title = jo["title"].Value<string>()

                });
                cfg.CreateMap<JObject, BookDetailsDto>().ConvertUsing<BookDetailConverter>();
                cfg.CreateMap<JObject, (string, IEnumerable<PlaceDto>)>().ConvertUsing<PlaceConverter>();
                cfg.CreateMap<JObject, IEnumerable<TutorDto>>().ConvertUsing<TutorMeConverter>();
            });
            return config;
        }
    }
}
