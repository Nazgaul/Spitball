using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Book;
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
        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            builder.RegisterModule<ModuleInfrastructureBase>();
            //builder.RegisterModule(new ModuleInfrastructureBase(_searchServiceCredentials, _redisConnectionString));

            builder.RegisterType<LuisAI>().As<IAi>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<AiDecision>().As<IDecision>();
            builder.RegisterType<EngineProcess>().As<IEngineProcess>();

            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Db;
                return new DapperRepository(key);
            });
            builder.Register(_ => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40")).AsImplementedInterfaces();

            builder.RegisterType<DocumentDbRepositoryUnitOfWork>().AsSelf().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).AsImplementedInterfaces();

            builder.RegisterType<BingSearch>().As<ISearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor), typeof(BuildLocalUrlInterceptor), typeof(ShuffleInterceptor));
            builder.RegisterType<Suggestions>().As<ISuggestions>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<ReplaceImageProvider>().AsSelf();

            builder.RegisterType<DocumentCseSearch>().As<IDocumentCseSearch>();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<CourseSearch>().As<ICourseSearch>();

            builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(BuildLocalUrlInterceptor));

            builder.RegisterType<VideoSearch>().As<IVideoSearch>();

            #region Job

            //builder.RegisterType<AzureJobSearch>().As<IJobProvider>();
            //builder.RegisterType<ZipRecruiterClient>().As<IJobProvider>();
            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(IJobProvider).IsAssignableFrom(w)).AsImplementedInterfaces();
            builder.RegisterType<JobSearch>().As<IJobSearch>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor), typeof(BuildLocalUrlInterceptor), typeof(ShuffleInterceptor));

            #endregion

            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<,>));
            builder.RegisterAssemblyTypes(currentAssembly).AsClosedTypesOf(typeof(IReadRepositoryAsync<>));

            builder.Register(c =>
            {
                var key = c.Resolve<IConfigurationKeys>().Storage;
                return new CloudStorageProvider(key);
            }).SingleInstance().AsImplementedInterfaces();

            builder.RegisterType<BlobProvider>().AsImplementedInterfaces();
            builder.RegisterType<QueueProvider>().AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(BlobProvider<>)).AsImplementedInterfaces();
        }
    }
}

