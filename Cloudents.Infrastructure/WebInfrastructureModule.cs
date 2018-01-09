using System.Collections.Generic;
using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Infrastructure
{
    public class WebInfrastructureModule : InfrastructureModule
    {
        public WebInfrastructureModule(string sqlConnectionString,
            string searchServiceName,
            string searchServiceKey,
            string redisConnectionString,
            string storageConnectionString) :
            base(sqlConnectionString, searchServiceName, searchServiceKey, redisConnectionString, storageConnectionString)
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<SeoFlashcardRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Flashcard);
            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Item);
            //builder.RegisterType<TutorMeSearch>().As<ITutorProvider>();

        }
    }
}
