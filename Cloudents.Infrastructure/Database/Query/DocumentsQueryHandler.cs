using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Domain.Entities;
using Cloudents.Domain.Enums;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
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
            return await _session.Query<Document>()
                .Fetch(f => f.User)
                .Fetch(f => f.University)
                .Where(w => ids.Contains(w.Id) && w.Item.State == ItemState.Ok)
                .Select(s => new DocumentFeedDto
                {
                    Id = s.Id,
                    User = new UserDto
                    {
                        Id = s.User.Id,
                        Name = s.User.Name,
                        Image = s.User.Image,
                        Score = s.User.Score
                    },
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Name,
                    TypeStr = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Snippet = s.MetaContent,
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Vote = new VoteDto()
                    {
                        Votes = s.Item.VoteCount
                    }
                })
                .ToListAsync(token);

        }
    }
}