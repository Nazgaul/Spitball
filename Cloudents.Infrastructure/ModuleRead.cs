using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Infrastructure.Auth;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search.Book;
using Cloudents.Infrastructure.Search.Job;
using Cloudents.Infrastructure.Search.Question;
using Cloudents.Infrastructure.Search.Tutor;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Search.Document;
using Cloudents.Infrastructure.Suggest;
using Cloudents.Query;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public sealed class ModuleRead : Module
    {
        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            // builder.RegisterType<DocumentDbRepositoryUnitOfWork>().AsSelf()
            //    /*.As<IStartable>()*/.SingleInstance()/*.AutoActivate()*/;
            // builder.RegisterGeneric(typeof(DocumentDbRepository<>)).AsImplementedInterfaces();

            //builder.RegisterType<BingSearch>().As<ISearch>().EnableInterfaceInterceptors()
            //    .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            //builder.RegisterType<DomainParser>().AsSelf().As<IDomainParser>().SingleInstance();
            //builder.RegisterType<DomainCache>().As<ICacheProvider>();

            //builder.RegisterType<ReplaceImageProvider>().As<IReplaceImageProvider>();

            //builder.RegisterType<WebSearch>();

            //builder.RegisterType<AzureQuestionSearch>().AsSelf();//
            builder.RegisterType<DapperRepository>().AsSelf();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();

            //builder.RegisterType<AzureDocumentSearch>().AsSelf();//
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();

            #region Tutor

            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(ITutorProvider).IsAssignableFrom(w)).AsImplementedInterfaces()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
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

            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<GoogleAuth>().As<IGoogleAuth>().SingleInstance();


            builder.RegisterType<BingSuggest>()
                .As<ISuggestions>()
                //.WithMetadata<SuggestMetadata>(m => m.For(am => am.AppenderName, Enum.GetValues(typeof(Vertical)).Cast<Vertical>().Where(w => w != Vertical.Tutor)))
                //.WithMetadata<SuggestMetadata>(m=>Enum.GetValues(typeof(Vertical)).Cast<Vertical>())
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
        }
    }
}

