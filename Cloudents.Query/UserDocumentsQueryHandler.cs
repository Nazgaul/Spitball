using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            return await _session.Query<ViewDocumentSearch>()
                .Where(w => w.UserId == query.Id)
                .OrderByDescending(o => o.Id)
                .Select(s => new DocumentFeedDto()
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
                    //Source = "Cloudents",
                    Views = s.Views,
                    Downloads = s.Downloads,
                    University = s.University,
                    Snippet = s.Snippet,
                    Price = s.Price,
                    Vote = new VoteDto
                    {
                        Votes = s.Votes
                    }

                }
                )
                .Take(50).Skip(query.Page * 50)
                .ToListAsync(token);
        }
    }


}