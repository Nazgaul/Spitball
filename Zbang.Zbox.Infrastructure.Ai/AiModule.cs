using Autofac;

namespace Zbang.Zbox.Infrastructure.Ai
{
    //public static class RegisterIoc
    //{
    //    public static void Register()
    //    {
    //        var ioc = IocFactory.IocWrapper;
    //        //ioc.RegisterType<ILuisAi, LuisAi>(LifeTimeManager.Singleton);
    //        ioc.RegisterType<IWitAi, WitAi>(LifeTimeManager.Singleton);
    //        ioc.RegisterType<IIntent, QuestionIntent>("AskQuestion");
    //        ioc.RegisterType<IIntent, SearchIntent>("SearchContent");
    //        //ioc.RegisterType<IIntent, ResearchIntent>("Research");
    //        //ioc.RegisterType<IIntent, JoinGroupIntent>("JoinGroup");
    //    }
    //}

    public class AiModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WitAi>().As<IWitAi>().SingleInstance();
            builder.RegisterType<QuestionIntent>().Named<IIntent>("AskQuestion");
            builder.RegisterType<SearchIntent>().Named<IIntent>("SearchContent");
        }
    }
}
