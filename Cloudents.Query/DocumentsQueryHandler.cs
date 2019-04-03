using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class DocumentsQueryHandler : IQueryHandler<IdsQuery<long>, IList<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public DocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IList<DocumentFeedDto>> GetAsync(IdsQuery<long> query, CancellationToken token)
        {
            var ids = query.QuestionIds.ToList();

            var z = await _session.Query<Document>()
                .Fetch(f => f.User)
                .Fetch(f => f.University)
                .Where(w => ids.Contains(w.Id) && w.Status.State == ItemState.Ok)
                .Select(s => new DocumentFeedDto
                {
                    Id = s.Id,
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Score = s.User.Score,
                        Image = s.User.Image
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Id,
                    Type = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Snippet = s.MetaContent,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Price = s.Price,
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