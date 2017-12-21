using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;
using Cloudents.Infrastructure.Search;

namespace Cloudents.Infrastructure
{
    public class WebInfrastructureModule : InfrastructureModule
    {
        public WebInfrastructureModule(string sqlConnectionString, string searchServiceName,
            string searchServiceKey, string redisConnectionString) :
            base(sqlConnectionString, searchServiceName, searchServiceKey, redisConnectionString)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SeoFlashcardRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Flashcard);
            builder.RegisterType<SeoQuizRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Quiz);

            builder.RegisterType<TutorMeSearch>().As<ITutorProvider>();
        }
    }
}
