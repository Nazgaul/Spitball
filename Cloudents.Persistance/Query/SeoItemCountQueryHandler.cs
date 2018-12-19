using Cloudents.Domain.Entities;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Enum;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Enums;

namespace Cloudents.Infrastructure.Database.Query
{
    public class SeoItemCountQueryHandler : IQueryHandler<EmptyQuery, IEnumerable<SiteMapCountDto>>
    {
        private readonly ISession _session;

        public SeoItemCountQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<SiteMapCountDto>> GetAsync(EmptyQuery query, CancellationToken token)
        {
            var documentCountFuture = _session.QueryOver<Document>()
                .Where(w => w.Item.State == ItemState.Ok)
                .ToRowCountQuery().FutureValue<int>();
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