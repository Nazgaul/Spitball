﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;

namespace Cloudents.Query
{
    public class SeoItemCountQueryHandler : IQueryHandler<EmptyQuery, IEnumerable<SiteMapCountDto>>
    {
        private readonly IStatelessSession _session;

        public SeoItemCountQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<SiteMapCountDto>> GetAsync(EmptyQuery query, CancellationToken token)
        {
            var documentCountFuture = _session.QueryOver<Document>()
                .Where(w => w.Status.State == ItemState.Ok)
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