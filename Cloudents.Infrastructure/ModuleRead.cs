using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Attributes;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Auth;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Domain;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search;
using Cloudents.Infrastructure.Search.Book;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Tutor;
using JetBrains.Annotations;
using BingSearch = Cloudents.Infrastructure.Search.BingSearch;
using ICacheProvider = Nager.PublicSuffix.ICacheProvider;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [ModuleRegistration(Core.Enum.System.Console)]
    [ModuleRegistration(Core.Enum.System.Web)]
    [UsedImplicitly]
    public sealed class ModuleRead : Module
    {
        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

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

            #region Job

            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(IJobProvider).IsAssignableFrom(w))
                .As<IJobProvider>()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(CacheResultInterceptor), typeof(LogInterceptor));
            builder.RegisterType<JobSearch>().As<IJobSearch>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor));

            #endregion

            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
            builder.RegisterType<DocumentIndexSearch>().AsImplementedInterfaces();
            builder.RegisterType<SearchConvertRepository>().AsImplementedInterfaces();

            builder.RegisterType<GoogleAuth>().As<IGoogleAuth>().SingleInstance();
        }
    }
}

