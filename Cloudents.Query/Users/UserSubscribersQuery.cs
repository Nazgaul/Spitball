using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core;
using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Users
{
    public class UserSubscribersQuery : IQuery<HashSet<long>>
    {
        public UserSubscribersQuery(long userId)
        {
            UserId = userId;
        }

        private long UserId { get; }

        internal sealed class UserSubscribersQueryHandler : IQueryHandler<UserSubscribersQuery, HashSet<long>>
        {
            private readonly IStatelessSession _session;

            public UserSubscribersQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }
            [Cache(TimeConst.Hour, "user-subscriber", false)]
            public async Task<HashSet<long>> GetAsync(UserSubscribersQuery query, CancellationToken token)
            {
                var result = await _session.Query<Follow>()
                    .Where(w => w.Follower.Id == query.UserId && w.Subscriber == true)
                    .Select(s => s.User.Id)
                    .ToListAsync(token);

                result.Add(query.UserId);
                return new HashSet<long>(result);
            }
        }
    }
}