using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class DocumentsQueryHandler : IQueryHandler<IdsDocumentsQuery<long>, IList<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public DocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<DocumentFeedDto>> GetAsync(IdsDocumentsQuery<long> query, CancellationToken token)
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
                        Score = s.UserScore,
                        Image = s.UserImage,
                        IsTutor = s.IsTutor
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