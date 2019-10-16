using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

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

        private IEnumerable<long> DocumentIds { get;  }


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

                var z = await _session.Query<ViewDocumentSearch>()
                    .Where(w => ids.Contains(w.Id))
                    .Select(s => new DocumentFeedDto
                    {
                        Id = s.Id,
                        User = new DocumentUserDto
                        {
                            Id = s.UserId,
                            Name = s.UserName,
                            Image = s.UserImage,
                        },
                        DateTime = s.DateTime,
                        Course = s.Course,
                        Title = s.Title,
                        Snippet = s.Snippet,
                        Views = s.Views,
                        Downloads = s.Downloads,
                        University = s.University,
                        Price = s.Price,
                        Purchased = s.Purchased,
                        DocumentType = s.DocumentType ?? DocumentType.Document,
                        Duration = s.Duration,
                        Vote = new VoteDto()
                        {
                            Votes = s.Votes
                        }
                    })
                    .ToListAsync(token);
                return z;
            }
        }
    }
}