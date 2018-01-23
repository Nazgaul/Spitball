using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Cloudents.Core.Entities.Search;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Cache;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Storage;
using Cloudents.Infrastructure.Write;
using Microsoft.Azure.Search;
using Microsoft.Cognitive.LUIS;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public class SearchServiceCredentials
    {
        public SearchServiceCredentials(string name, string key)
        {
            Name = name;
            Key = key;
        }

        public string Name { get;  }
        public string Key { get;  }
    }
    public class ModuleInfrastructureBase : Module
    {
        private readonly SearchServiceCredentials _searchService;

        public ModuleInfrastructureBase(SearchServiceCredentials searchService)
        {
            _searchService = searchService;
        }


        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new SearchServiceClient(_searchService.Name, new SearchCredentials(_searchService.Key)))
                    .SingleInstance().AsSelf().As<ISearchServiceClient>();
        }
    }


    public class WriteModuleBase : ModuleInfrastructureBase
    {

        public WriteModuleBase(SearchServiceCredentials searchCredentials) :base(searchCredentials)
        {
        }
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterGeneric(typeof(SearchServiceWrite<>));
            builder.RegisterType<JobSearchWrite>().As<ISearchServiceWrite<Job>>().As<IStartable>().SingleInstance().AutoActivate();
        }
    }


    public class InfrastructureModule : Module
    {
        protected readonly string SqlConnectionString;
        private readonly string _searchServiceName;
        private readonly string _searchServiceKey;
        protected readonly string RedisConnectionString;

        private readonly string _storageConnectionString;

        public InfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString, string storageConnectionString
            )
        {
            SqlConnectionString = sqlConnectionString;
            _searchServiceName = searchServiceName;
            _searchServiceKey = searchServiceKey;
            RedisConnectionString = redisConnectionString;
            _storageConnectionString = storageConnectionString;
        }

        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
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

            builder.Register(c => new DapperRepository(SqlConnectionString));
            builder.Register(c => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40")).AsImplementedInterfaces();

            builder.RegisterType<DocumentDbRepositoryUnitOfWork>().AsSelf().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).AsImplementedInterfaces();



            builder.RegisterType<CacheProvider>().AsImplementedInterfaces();
            builder.RegisterType<BingSearch>().As<ISearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentCseSearch>().As<IDocumentCseSearch>();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>();
            builder.RegisterType<CourseSearch>().As<ICourseSearch>();
            builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>();

            builder.RegisterType<VideoSearch>().As<IVideoSearch>();
            builder.RegisterType<JobSearch>().As<IJobSearch>();
            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<RestClient>().As<IRestClient>();
            builder.RegisterType<GooglePlacesSearch>().As<IGooglePlacesSearch>();
            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));

            builder.RegisterGeneric(typeof(EfRepository<>)).AsImplementedInterfaces();

            builder.Register(c => new CloudStorageProvider(_storageConnectionString)).SingleInstance();
            builder.RegisterType<BlobProvider>().AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(BlobProvider<>)).AsImplementedInterfaces();

            var config = MapperConfiguration();
            builder.Register(c => config.CreateMapper()).SingleInstance();
        }

        private static MapperConfiguration MapperConfiguration()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<MapperProfile>());
        }
    }
}

