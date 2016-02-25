﻿using Zbang.Zbox.Infrastructure.Ioc;

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

            ioc.RegisterType<IBoxWriteSearchProvider2, BoxSearchProvider>(LifeTimeManager.Singleton);
            ioc.RegisterType<IBoxReadSearchProvider2, BoxSearchProvider>(LifeTimeManager.Singleton);

            ioc.RegisterType<IItemWriteSearchProvider3, ItemSearchProvider3>(LifeTimeManager.Singleton);
            ioc.RegisterType<IItemReadSearchProvider2, ItemSearchProvider3>(LifeTimeManager.Singleton);


            ioc.RegisterType<IQuizWriteSearchProvider2, QuizSearchProvider2>(LifeTimeManager.Singleton);
            ioc.RegisterType<IQuizReadSearchProvider2, QuizSearchProvider2>(LifeTimeManager.Singleton);



        }
    }
}
