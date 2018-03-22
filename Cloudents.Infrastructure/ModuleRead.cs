using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Read;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Domain;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Book;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Places;
using Cloudents.Infrastructure.Search.Tutor;
using Microsoft.Cognitive.LUIS;
using BingSearch = Cloudents.Infrastructure.Search.BingSearch;
using ICacheProvider = Nager.PublicSuffix.ICacheProvider;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Api)]
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

            builder.Register(_ => new LuisClient("a1a0245f-4cb3-42d6-8bb2-62b6cfe7d5a3", "6effb3962e284a9ba73dfb57fa1cfe40")).AsImplementedInterfaces();

            builder.RegisterType<DocumentDbRepositoryUnitOfWork>().AsSelf().As<IStartable>().SingleInstance().AutoActivate();
            builder.RegisterGeneric(typeof(DocumentDbRepository<>)).AsImplementedInterfaces();

            builder.RegisterType<BingSearch>().As<ISearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            builder.RegisterType<DomainParser>().AsSelf().As<IDomainParser>().SingleInstance();
            builder.RegisterType<DomainCache>().As<ICacheProvider>();


           
            builder.RegisterType<ReplaceImageProvider>().As<IReplaceImageProvider>();


            builder.RegisterType<WebSearch>();

            builder.RegisterType<CourseSearch>().As<ICourseSearch>();

            #region Tutor

            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(ITutorProvider).IsAssignableFrom(w)).AsImplementedInterfaces();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(BuildLocalUrlInterceptor));

            #endregion

            builder.RegisterType<VideoSearch>().As<IVideoSearch>();

            #region Job

            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(IJobProvider).IsAssignableFrom(w))
                .As<IJobProvider>()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<JobSearch>().As<IJobSearch>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor));

            #endregion

            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            builder.RegisterType<PlacesSearch>().As<IPlacesSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterModule<ModuleReadDb>();

        }
    }
}

