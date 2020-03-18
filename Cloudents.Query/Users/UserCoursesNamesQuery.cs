using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserCoursesNamesQuery : IQuery<IEnumerable<string>>
    {
        public UserCoursesNamesQuery(long userId)
        {
            UserId = userId;
        }
        public long UserId { get; }

        internal sealed class UserCoursesNamesQueryHandler : IQueryHandler<UserCoursesNamesQuery, IEnumerable<string>>
        {
            private readonly IStatelessSession _session;

            public UserCoursesNamesQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<string>> GetAsync(UserCoursesNamesQuery query, CancellationToken token)
            {
                return await _session.Query<UserCourse>()
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Course.Id)
                    .ToListAsync(token);
            }
        }
    }
}
