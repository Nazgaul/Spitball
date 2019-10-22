using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Query
{
    public class UserPurchaseDocumentByIdQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public UserPurchaseDocumentByIdQuery(long id, int page)
        {
            Id = id;
            Page = page;
        }

        private long Id { get; }
        private int Page { get; }


        internal sealed class UserPurchasedDocumentsQueryHandler : IQueryHandler<UserPurchaseDocumentByIdQuery, IEnumerable<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;

            public UserPurchasedDocumentsQueryHandler(QuerySession repository)
            {
                _session = repository.StatelessSession;
            }
            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserPurchaseDocumentByIdQuery query, CancellationToken token)
            {

                return await _session.Query<Document>()
                    .Fetch(f=>f.User)
                    .ThenFetch(f=>f.University)
                    .Join(_session.Query<DocumentTransaction>(), l => l.Id, r => r.Document.Id, (search, transaction) =>
                          new
                          {
                              search,
                              transaction
                          })
                    .Where(w => w.transaction.User.Id == query.Id && w.transaction.Type == TransactionType.Spent)
                    .OrderByDescending(o=>o.transaction.Created)
                    .Select(s => new DocumentFeedDto
                    {
                        Id = s.search.Id,
                        User = new DocumentUserDto
                        {
                            Id = s.search.User.Id,
                            Name = s.search.User.Name,
                            Image = s.search.User.Image,
                        },
                        DateTime = s.search.TimeStamp.UpdateTime,
                        Course = s.search.Course.Id,
                        Title = s.search.Name,
                        Snippet = s.search.MetaContent,
                        Views = s.search.Views,
                        Downloads = s.search.Downloads,
                        University = s.search.User.University.Name,
                        Price = s.search.Price,
                        Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.search.Id && x.Action == TransactionActionType.SoldDocument),
                        DocumentType = s.search.DocumentType ?? DocumentType.Document,
                        Duration = s.search.Duration,
                        Vote = new VoteDto()
                        {
                            Votes = s.search.VoteCount
                        }
                    })
                    
                    .Take(50).Skip(query.Page * 50)

                    .ToListAsync(token);
            }

        }
    }
}