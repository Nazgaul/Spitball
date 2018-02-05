using System.Collections.Generic;
using Autofac;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Request;
using Cloudents.Infrastructure.Data;

namespace Cloudents.Infrastructure
{
    public class ModuleWeb : Module
    {
        private readonly string _sqlConnectionString;
        private readonly SearchServiceCredentials _searchServiceCredentials;
        private readonly string _redisConnectionString;
        private readonly string _storageConnectionString;

        public ModuleWeb(string sqlConnectionString,
            SearchServiceCredentials searchServiceCredentials,
            string redisConnectionString,
            string storageConnectionString)
        {
            _sqlConnectionString = sqlConnectionString;
            _searchServiceCredentials = searchServiceCredentials;
            _redisConnectionString = redisConnectionString;
            _storageConnectionString = storageConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterModule(new ModuleRead(_sqlConnectionString, _redisConnectionString,
                _storageConnectionString, _searchServiceCredentials));
            builder.RegisterType<SeoFlashcardRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Flashcard);
            builder.RegisterType<SeoDocumentRepository>()
                .Keyed<IReadRepository<IEnumerable<SiteMapSeoDto>, SeoQuery>>(SeoType.Item);
        }
    }
}
