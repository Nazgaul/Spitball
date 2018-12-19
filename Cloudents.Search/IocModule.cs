using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Search.Document;
using Cloudents.Search.Question;
using Cloudents.Search.University;
using Module = Autofac.Module;

namespace Cloudents.Search
{
    public class SearchModule : Module
    {

        public SearchModule(string name, string key, bool isDevelop)
        {
            Name = name;
            Key = key;
            IsDevelop = isDevelop;
        }

        private string Name { get; }
        private string Key { get; }

        private bool IsDevelop { get; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureQuestionSearch>().As<IQuestionsSearch>().AsSelf();
            builder.RegisterType<AzureDocumentSearch>().AsSelf().As<IDocumentsSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();

            builder.RegisterType<DocumentSearchWrite>().As<SearchServiceWrite<Entities.Document>>();
            //builder.RegisterGeneric(typeof(SearchServiceWrite<>)).AsSelf();
            //var assembly = Assembly.GetExecutingAssembly();
            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>))
            //    .AsImplementedInterfaces();


            builder.Register(c=> new SearchService(Key,Name,IsDevelop)).AsSelf().As<ISearchService>().SingleInstance();
            base.Load(builder);
        }
    }
}
