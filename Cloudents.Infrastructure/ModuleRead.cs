using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Search.Tutor;
using Cloudents.Infrastructure.Storage;
using Microsoft.Cognitive.LUIS;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public sealed class ModuleRead : Module
    {
        private readonly string _sqlConnectionString;
        private readonly SearchServiceCredentials _searchServiceCredentials;

        private readonly string _redisConnectionString;

        private readonly string _storageConnectionString;

        public ModuleRead(string sqlConnectionString,
            string redisConnectionString, string storageConnectionString,
            SearchServiceCredentials searchServiceCredentials)
        {
            _sqlConnectionString = sqlConnectionString;
            _redisConnectionString = redisConnectionString;
            _storageConnectionString = storageConnectionString;
            _searchServiceCredentials = searchServiceCredentials;
        }

        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new ModuleInfrastructureBase(_searchServiceCredentials, _redisConnectionString));

            builder.RegisterType<LuisAI>().As<IAI>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<EngineProcess>().As<IEngineProcess>();

            builder.Register(_ => new DapperRepository(_sqlConnectionString));
            builder.Register(_ => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40")).AsImplementedInterfaces();

            builder.RegisterType<DocumentDbRepositoryUnitOfWork>().AsSelf().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).AsImplementedInterfaces();

            builder.RegisterType<BingSearch>().As<ISearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<ReplaceImageProvider>().AsSelf();

            builder.RegisterType<DocumentCseSearch>().As<IDocumentCseSearch>();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>();
            builder.RegisterType<CourseSearch>().As<ICourseSearch>();
            builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>();

            builder.RegisterType<VideoSearch>().As<IVideoSearch>();

            #region Job

            builder.RegisterType<AzureJobSearch>().As<IJobProvider>();
            builder.RegisterType<ZipRecruiterClient>().As<IJobProvider>();
            builder.RegisterType<JobSearch>().As<IJobSearch>();

            #endregion

            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));

            //builder.RegisterGeneric(typeof(EfRepository<>)).AsImplementedInterfaces();

            builder.Register(_ => new CloudStorageProvider(_storageConnectionString)).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<BlobProvider>().AsImplementedInterfaces();
            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(BlobProvider<>)).AsImplementedInterfaces();

            builder.RegisterType<StreamSerializer>().AsImplementedInterfaces();
        }
    }
}

