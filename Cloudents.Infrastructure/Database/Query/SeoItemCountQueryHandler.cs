using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query
{
    public class SeoItemCountQueryHandler : IQueryHandler<EmptyQuery, IEnumerable<SiteMapCountDto>>
    {
        private readonly ISession _session;

        public SeoItemCountQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<SiteMapCountDto>> GetAsync(EmptyQuery query, CancellationToken token)
        {
            var documentCountFuture = _session.QueryOver<Document>().ToRowCountQuery().FutureValue<int>();
            var flashcardCountQueryFuture = _session.CreateSQLQuery(
                @"select sum(b.FlashcardCount) as flashcardCount
from zbox.box b 
where University in (select id from zbox.University where  needcode = 0)
and Discriminator = 2
and IsDeleted = 0").FutureValue<int>();

            var documentCount = await documentCountFuture.GetValueAsync(token);
            var flashcardCount = flashcardCountQueryFuture.Value;
            return new List<SiteMapCountDto>
            {
                new SiteMapCountDto(SeoType.Item, documentCount),
                new SiteMapCountDto(SeoType.Flashcard, flashcardCount),
            };
        }
    }
}