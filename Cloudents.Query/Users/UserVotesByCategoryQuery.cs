using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserVotesByCategoryQuery : IQuery<IEnumerable<UserVoteDocumentDto>>
    {
        public UserVotesByCategoryQuery(long userId)
        {
            UserId = userId;
        }

        public long UserId { get; }

        internal sealed class UserVotesByCategoryCommandHandler : IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>
        {
            private readonly IStatelessSession _session;

            public UserVotesByCategoryCommandHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<UserVoteDocumentDto>> GetAsync(UserVotesByCategoryQuery query, CancellationToken token)
            {
                return await _session.Query<Vote>()
                        .Where(w => w.Document != null && w.User.Id == query.UserId)
                        .Select(s => new UserVoteDocumentDto
                        {
                            Id = s.Document.Id,
                            Vote = s.VoteType
                        }).ToListAsync(token);
            }

        }
    }
}
