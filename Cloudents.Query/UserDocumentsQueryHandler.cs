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
    public class UserDocumentsQueryHandler : IQueryHandler<UserDataPagingByIdQuery, IEnumerable<DocumentFeedDto>>
    {
        private readonly IStatelessSession _session;

        public UserDocumentsQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }
        public async Task<IEnumerable<DocumentFeedDto>> GetAsync(UserDataPagingByIdQuery query, CancellationToken token)
        {
            return await _session.Query<Document>()
                .Fetch(f => f.University)
                .Fetch(f => f.User)
                .Where(w => w.User.Id == query.Id && w.Item.State == ItemState.Ok)
                .OrderByDescending(o => o.Id)
                .Select(s => new DocumentFeedDto()
                {
                    Id = s.Id,
                    User = new UserDto(s.User.Id, s.User.Name, s.User.Score),
                    //{
                    //    Id = s.User.Id,
                    //    Name = s.User.Name,
                    //    Image = s.User.Image,
                    //    Score = s.User.Score
                    //},
                    DateTime = s.TimeStamp.UpdateTime,
                    Course = s.Course.Name,
                    TypeStr = s.Type,
                    Professor = s.Professor,
                    Title = s.Name,
                    Source = "Cloudents",
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University.Name,
                    Snippet = s.MetaContent,
                    Vote = new VoteDto()
                    {
                        Votes = s.Item.VoteCount
                    }

                }
                )
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }
}