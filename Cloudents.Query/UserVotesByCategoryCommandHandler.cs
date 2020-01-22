using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Users;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    public class UserVotesByCategoryCommandHandler :
        IQueryHandler<UserVotesByCategoryQuery, IEnumerable<UserVoteDocumentDto>>
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