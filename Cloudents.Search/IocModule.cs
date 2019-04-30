using Autofac;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Search.Document;
using Cloudents.Search.Question;
using Cloudents.Search.Tutor;
using Cloudents.Search.University;
using Module = Autofac.Module;

namespace Cloudents.Search
{
    public class SearchModule : Module
    {

        //public SearchModule(string name, string key, bool isDevelop)
        //{
        //    Name = name;
        //    Key = key;
        //    IsDevelop = isDevelop;
        //}

        //private string Name { get; }
        //private string Key { get; }

        //private bool IsDevelop { get; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureQuestionSearch>().As<IQuestionsSearch>().AsSelf();
            builder.RegisterType<AzureDocumentSearch>().AsSelf().As<IDocumentsSearch>();
            builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();

            builder.RegisterType<DocumentSearchWrite>().As<SearchServiceWrite<Entities.Document>>();
            builder.RegisterType<UniversitySearchWrite>().AsSelf();
            builder.RegisterType<QuestionSearchWrite>().AsSelf();
            builder.RegisterType<DocumentSearchWrite>().AsSelf();
            //builder.RegisterGeneric(typeof(SearchServiceWrite<>)).AsSelf();
            //var assembly = Assembly.GetExecutingAssembly();
            //builder.RegisterAssemblyTypes(assembly).AsClosedTypesOf(typeof(ISearchServiceWrite<>))
            //    .AsImplementedInterfaces();

            //builder.RegisterType<TutorAzureSearch>().As<ITutorProvider>().EnableInterfaceInterceptors()
            //    .InterceptedBy(typeof(LogInterceptor));


            //builder.RegisterType<AzureJobSearch>().As<IJobProvider>().EnableInterfaceInterceptors()
            //    .InterceptedBy(typeof(CacheResultInterceptor), typeof(LogInterceptor));


            builder.Register(c=>
            {
                var configuration = c.Resolve<IConfigurationKeys>().Search;
                return new SearchService(configuration.Key, configuration.Name, configuration.IsDevelop);
            }).AsSelf().As<ISearchService>().SingleInstance();
            base.Load(builder);


            builder.RegisterType<TutorSuggest>().Keyed<ISuggestions>(Vertical.Tutor).WithParameter(TutorSuggest.VerticalParameter, Vertical.Tutor);
            builder.RegisterType<TutorSuggest>().Keyed<ISuggestions>(Vertical.Job).WithParameter(TutorSuggest.VerticalParameter, Vertical.Job);
            builder.RegisterType<TutorSuggest>().As<ITutorSuggestion>().WithParameter(TutorSuggest.VerticalParameter, Vertical.Tutor);
        }
    }
}
