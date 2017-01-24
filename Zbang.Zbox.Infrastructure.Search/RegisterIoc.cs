using Autofac;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Search
{
    public class SearchModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<SeachConnection>()
                .As<ISearchConnection>()
                .WithParameter("serviceName", ConfigFetcher.Fetch("AzureSeachServiceName"))
                .WithParameter("serviceKey", ConfigFetcher.Fetch("AzureSearchKey")).SingleInstance();

            builder.RegisterType<UniversitySearchProvider>().As<IUniversityReadSearchProvider>().As<IUniversityWriteSearchProvider2>().SingleInstance();
            builder.RegisterType<FilterProvider>().As<ISearchFilterProvider>().SingleInstance();
            builder.RegisterType<BoxSearchProvider>().As<IBoxWriteSearchProvider2>().As<IBoxReadSearchProvider2>().SingleInstance();
            builder.RegisterType<ItemSearchProvider3>().As<IItemWriteSearchProvider>().As<IItemReadSearchProvider>().SingleInstance();
            builder.RegisterType<QuizSearchProvider2>().As<IQuizWriteSearchProvider2>().As<IQuizReadSearchProvider2>().SingleInstance();
            builder.RegisterType<FlashcardSearchProvider>().As<IFlashcardWriteSearchProvider>().As<IFlashcardReadSearchProvider>().SingleInstance();
            builder.RegisterType<ContentSearchProvider>().As<IContentWriteSearchProvider>().SingleInstance();
        }
    }
}
