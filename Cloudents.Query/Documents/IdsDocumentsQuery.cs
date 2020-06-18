﻿using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    /// <summary>
    /// This query is for search purposes
    /// </summary>
    public class IdsDocumentsQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public IdsDocumentsQuery(IEnumerable<long> ids)
        {
            DocumentIds = ids;
        }

        private IEnumerable<long> DocumentIds { get; }


        internal sealed class DocumentsQueryHandler : IQueryHandler<IdsDocumentsQuery, IEnumerable<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;

            public DocumentsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(IdsDocumentsQuery query, CancellationToken token)
            {
                var ids = query.DocumentIds.ToList();

                var z = await _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(IdsDocumentsQuery)))
                    .Fetch(f => f.User)
                    .Where(w => ids.Contains(w.Id) && w.Status.State == ItemState.Ok)
                    
                    .Select(s => new DocumentFeedDto
                    {
                        Id = s.Id,
                        User = new DocumentUserDto
                        {
                            Id = s.User.Id,
                            Name = s.User.Name,
                            Image = s.User.ImageName,
                        },
                        DateTime = s.TimeStamp.UpdateTime,
                        Course = s.Course.Id,
                        Title = s.Name,
                        Snippet = s.Description ?? s.MetaContent,
                       // Views = s.Views,
                       // Downloads = s.Downloads,
                        Price = s.DocumentPrice.Price,
                       // Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument),
                        DocumentType = s.DocumentType,
                        Duration = s.Duration,
                        PriceType = s.DocumentPrice.Type ?? PriceType.Free,
                        Vote = new VoteDto()
                        {
                            Votes = s.VoteCount
                        }
                    }).ToListAsync(token);
                return z;
            }
        }
    }
}