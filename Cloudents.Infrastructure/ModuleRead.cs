using Autofac;
using Autofac.Extras.DynamicProxy;
using Cloudents.Core.Interfaces;
using Cloudents.Infrastructure.Google;
using Cloudents.Infrastructure.Interceptor;
using Cloudents.Infrastructure.Search.Document;
using System.Diagnostics.CodeAnalysis;
using Module = Autofac.Module;

namespace Cloudents.Infrastructure
{
    public sealed class ModuleRead : Module
    {
        [SuppressMessage("Microsoft.Design", "RCS1163:Unused parameter")]
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>();
            builder.RegisterType<FeedService>().As<IFeedService>();


            builder.RegisterType<DocumentFeedService>().AsSelf().Keyed<IFeedTypeService>(Core.Enum.FeedType.Document);
            builder.RegisterType<TutorFeedService>().AsSelf().Keyed<IFeedTypeService>(Core.Enum.FeedType.Tutor);
            builder.RegisterType<QuestionFeedService>().Keyed<IFeedTypeService>(Core.Enum.FeedType.Question);
            builder.RegisterType<DocumentFeedService>().AsSelf().Keyed<IFeedTypeService>(Core.Enum.FeedType.Video);
            builder.RegisterType<AggregateFeedService>().Keyed<IFeedTypeService>(Core.Enum.FeedType.All);

            builder.RegisterType<IpToLocation>().As<IIpToLocation>().EnableInterfaceInterceptors()
                .InterceptedBy(typeof(CacheResultInterceptor));

            builder.RegisterType<GoogleService>().AsSelf()
                .As<IGoogleDocument>()
                .As<IGoogleAuth>().As<IGoogleAnalytics>()
                .As<ICalendarService>().SingleInstance();


            builder.RegisterType<GoogleDataStore>()
                .AsSelf().InstancePerDependency();



        }
    }
}

