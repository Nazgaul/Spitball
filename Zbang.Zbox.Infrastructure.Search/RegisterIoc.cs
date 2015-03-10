using Zbang.Zbox.Infrastructure.Ioc;

namespace Zbang.Zbox.Infrastructure.Search
{
    public static class RegisterIoc
    {
        public static void Register()
        {
            var ioc = IocFactory.IocWrapper;
          


            ioc.RegisterType<IUniversityReadSearchProvider, UniversitySearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IUniversityWriteSearchProvider2, UniversitySearchProvider>(LifeTimeManager.Singleton);

            ioc.RegisterType<ISearchFilterProvider, FilterProvider>(LifeTimeManager.Singleton);

            ioc.RegisterType<IBoxWriteSearchProvider, BoxSearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IBoxReadSearchProvider, BoxSearchProvider>(LifeTimeManager.Singleton);

            //ioc.RegisterType<IItemWriteSearchProvider, ItemSearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IItemWriteSearchProvider2, ItemSearchProvider2>(LifeTimeManager.Singleton);
            ioc.RegisterType<IItemReadSearchProvider, ItemSearchProvider2>(LifeTimeManager.Singleton);

            ioc.RegisterType<IQuizWriteSearchProvider, QuizSearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IQuizReadSearchProvider, QuizSearchProvider>(LifeTimeManager.Singleton);



        }
    }
}
