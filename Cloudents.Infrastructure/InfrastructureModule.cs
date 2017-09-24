using Autofac;
using AutoMapper;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.AI;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;
using Microsoft.Azure.Search;

namespace Cloudents.Infrastructure
{
    public class InfrastructureModule : Module
    {
        private readonly string m_SqlConnectionString;
        private readonly string m_SearchServiceName;
        private readonly string m_SearchServiceKey;

        public InfrastructureModule(string sqlConnectionString, string searchServiceName, string searchServiceKey)
        {
            m_SqlConnectionString = sqlConnectionString;
            m_SearchServiceName = searchServiceName;
            m_SearchServiceKey = searchServiceKey;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LuisAI>().As<IAI>();
            builder.RegisterType<AIDecision>().As<IDecision>();
            builder.RegisterType<UniqueKeyGenerator>().As<IKeyGenerator>();


            builder.Register(c => new DapperRepository(m_SqlConnectionString));
            builder.Register(c =>
                new SearchServiceClient(m_SearchServiceName, new SearchCredentials(m_SearchServiceKey))).SingleInstance();


            builder.RegisterType<DocumentSearch>().As<IDocumentSearch>().PropertiesAutowired();
            builder.RegisterType<FlashcardSearch>().As<IFlashcardSearch>().PropertiesAutowired();
            builder.RegisterType<QuestionSearch>().As<IQuestionSearch>().PropertiesAutowired();
            builder.RegisterType<TutorSearch>().As<ITutorSearch>().PropertiesAutowired();


            builder.RegisterType<UniversitySynonymRepository>().As<IReadRepositorySingle<UniversitySynonymDto, long>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Search.Entities.Tutor, TutorDto>()
                    .ForMember(d => d.Latitude, o => o.ResolveUsing(p => p.Location.Latitude))
                    .ForMember(d => d.Longitude, o => o.ResolveUsing(p => p.Location.Longitude));

                //cfg.CreateMap<Search.Entities.Tutor, Cloudents.Core.DTOs.TutorDto>()
            });
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<Search.Entities.Tutor, Cloudents.Core.DTOs.TutorDto>()
            //        .ForMember(d => d.Latitude, o => o.ResolveUsing(p => p.Location.Latitude))
            //        .ForMember(d => d.Longitude, o => o.ResolveUsing(p => p.Location.Longitude));

            //    //cfg.CreateMap<Search.Entities.Tutor, Cloudents.Core.DTOs.TutorDto>()
            //});
            builder.Register(c => config.CreateMapper()).SingleInstance();


        }
    }
}
