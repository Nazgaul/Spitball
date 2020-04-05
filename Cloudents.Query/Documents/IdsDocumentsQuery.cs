using Cloudents.Core.DTOs.Documents;
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
    public class IdsDocumentsQuery : IQuery<IList<DocumentFeedDto>>
    {
        public IdsDocumentsQuery(IEnumerable<long> ids)
        {
            DocumentIds = ids;
        }

        private IEnumerable<long> DocumentIds { get; }


        internal sealed class DocumentsQueryHandler : IQueryHandler<IdsDocumentsQuery, IList<DocumentFeedDto>>
        {
            private readonly IStatelessSession _session;

            public DocumentsQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IList<DocumentFeedDto>> GetAsync(IdsDocumentsQuery query, CancellationToken token)
            {
                var ids = query.DocumentIds.ToList();

                var z = await _session.Query<Document>()
                    .WithOptions(w => w.SetComment(nameof(IdsDocumentsQuery)))
                    .Fetch(f => f.User)
                    .ThenFetch(f => f.University)
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
                        Views = s.Views,
                        Downloads = s.Downloads,
                        University = s.University.Name,
                        Price = s.Price,
                        Purchased = _session.Query<DocumentTransaction>().Count(x => x.Document.Id == s.Id && x.Action == TransactionActionType.SoldDocument),
                        DocumentType = s.DocumentType ?? DocumentType.Document,
                        Duration = s.Duration,
                        Vote = new VoteDto()
                        {
                            Votes = s.VoteCount
                        }
                    })
                    .ToListAsync(token);
                return z;
            }
        }
    }
}