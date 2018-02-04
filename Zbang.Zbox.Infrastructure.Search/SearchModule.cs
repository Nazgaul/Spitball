using Autofac;
using Zbang.Zbox.Infrastructure.Extensions;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SearchModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SearchConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey")).SingleInstance();

            builder.RegisterType<UniversitySearchProvider>().As<IUniversityWriteSearchProvider2>().SingleInstance();
            builder.RegisterType<FilterProvider>().As<ISearchFilterProvider>().SingleInstance();
            builder.RegisterType<BoxSearchProvider>().As<IBoxWriteSearchProvider2>().SingleInstance();
            builder.RegisterType<ItemSearchProvider3>().As<IItemWriteSearchProvider>().SingleInstance();
            builder.RegisterType<QuizSearchProvider2>().As<IQuizWriteSearchProvider2>().SingleInstance();
            builder.RegisterType<FlashcardSearchProvider>().As<IFlashcardWriteSearchProvider>().SingleInstance();
            builder.RegisterType<ContentSearchProvider>().As<IContentWriteSearchProvider>().SingleInstance();
        }
    }
}
