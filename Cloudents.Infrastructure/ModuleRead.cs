using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Infrastructure.Auth;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search.Question;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
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
           
            builder.RegisterType<DapperRepository>().AsSelf();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>();
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();

            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<GoogleService>().As<IGoogleDocument>().As<IGoogleAuth>().SingleInstance();

            builder.RegisterType<BingSuggest>()
                .As<ISuggestions>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));
        }
    }
}

