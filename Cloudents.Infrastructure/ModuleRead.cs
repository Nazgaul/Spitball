using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Infrastructure.Interceptor;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Google;
using Cloudents.Infrastructure.Search.Document;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    [UsedImplicitly]
    public sealed class ModuleRead : Module
    {
        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();

            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<GoogleService>().AsSelf()
                .As<IGoogleDocument>()
                .As<IGoogleAuth>()
                .As<ICalendarService>().SingleInstance();


            builder.RegisterType<GoogleDataStore>()
                .AsSelf().InstancePerDependency();



        }
    }
}

