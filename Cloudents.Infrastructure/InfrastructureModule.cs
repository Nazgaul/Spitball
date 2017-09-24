using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LuisAI>().As<IAI>();
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();

            
            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>().PropertiesAutowired();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>().PropertiesAutowired();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>().PropertiesAutowired();

            
            builder.RegisterType<UniversitySynonymRepository>().As<IReadRepositorySingle<UniversitySynonymDto, long>>();
        }
    }
}
