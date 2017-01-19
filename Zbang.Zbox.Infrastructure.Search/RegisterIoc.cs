using Autofac;
using Zbang.Zbox.Infrastructure.Extensions;
using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Search
{
    //public static class RegisterIoc
    //{
        //public static void Register()
        //{
        //    var ioc = IocFactory.IocWrapper;

        //    ioc.RegisterType<IUniversityReadSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);
        //    ioc.RegisterType<IUniversityWriteSearchProvider2, UniversitySearchProvider>(LifeTimeManager.Singleton);

        //    ioc.RegisterType<ISearchFilterProvider, FilterProvider>(LifeTimeManager.Singleton);

        //    ioc.RegisterType<IBoxWriteSearchProvider2, BoxSearchProvider>(LifeTimeManager.Singleton);
        //    ioc.RegisterType<IBoxReadSearchProvider2, BoxSearchProvider>(LifeTimeManager.Singleton);

        //    ioc.RegisterType<IItemWriteSearchProvider, ItemSearchProvider3>(LifeTimeManager.Singleton);
        //    ioc.RegisterType<IItemReadSearchProvider, ItemSearchProvider3>(LifeTimeManager.Singleton);


        //    ioc.RegisterType<IQuizWriteSearchProvider2, QuizSearchProvider2>(LifeTimeManager.Singleton);
        //    ioc.RegisterType<IQuizReadSearchProvider2, QuizSearchProvider2>(LifeTimeManager.Singleton);

        //    ioc.RegisterType<IFlashcardWriteSearchProvider, FlashcardSearchProvider>(LifeTimeManager.Singleton);
        //    ioc.RegisterType<IFlashcardReadSearchProvider, FlashcardSearchProvider>(LifeTimeManager.Singleton);

        //}

        
    //}
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
        }
    }
}
