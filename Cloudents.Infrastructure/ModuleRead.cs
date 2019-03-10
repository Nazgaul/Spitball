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
           
            builder.RegisterType<DapperRepository>().AsSelf();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();

            #region Tutor

            builder.RegisterAssemblyTypes(currentAssembly)
                .Where(w => typeof(ITutorProvider).IsAssignableFrom(w)).AsImplementedInterfaces()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(LogInterceptor));
            builder.RegisterType<TutorSearch>().As<ITutorSearch>()
                .EnableInterfaceInterceptors().InterceptedBy(typeof(BuildLocalUrlInterceptor));

            #endregion
            

            builder.RegisterType<BookSearch>().As<IBookSearch>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(BuildLocalUrlInterceptor), typeof(CacheResultInterceptor));

            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<GoogleAuth>().As<IGoogleAuth>().SingleInstance();

            builder.RegisterType<BingSuggest>()
                .As<ISuggestions>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
        }
    }
}

