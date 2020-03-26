using Autofac;
using Cloudents.Core.Interfaces;
using Cloudents.Search.Document;
using Cloudents.Search.Tutor;
using Module = Autofac.Module;

namespace Cloudents.Search
{
    public class SearchModule : Module
    {

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AzureDocumentSearch>().AsSelf().As<IDocumentsSearch>();
            //builder.RegisterType<UniversitySearch>().As<IUniversitySearch>();
            builder.RegisterType<AzureTutorSearch>().AsSelf().As<ITutorSearch>();

            builder.RegisterType<DocumentSearchWrite>().As<SearchServiceWrite<Entities.Document>>();
           // builder.RegisterType<UniversitySearchWrite>().AsSelf();
            builder.RegisterType<DocumentSearchWrite>().AsSelf();
            builder.RegisterType<TutorSearchWrite>().AsSelf();



            builder.Register(c =>
            {
                var configuration = c.Resolve<IConfigurationKeys>().Search;
                return new SearchService(configuration.Key, configuration.Name, configuration.IsDevelop);
            }).AsSelf().As<ISearchService>().SingleInstance();
            base.Load(builder);


        }
    }
}
