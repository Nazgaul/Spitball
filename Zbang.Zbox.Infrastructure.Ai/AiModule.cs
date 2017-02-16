using Autofac;

namespace Zbang.Zbox.Infrastructure.Ai
{
    public class AiModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            //builder.RegisterType<WitAi>().As<IWitAi>().SingleInstance();
            //builder.RegisterType<QuestionIntent>().Named<IIntent>("AskQuestion");
            //builder.RegisterType<SearchIntent>().Named<IIntent>("SearchContent");
        }
    }
}
