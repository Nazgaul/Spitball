﻿using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
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

        private long UserId { get; }

        internal sealed class UserCoursesNamesQueryHandler : IQueryHandler<UserCoursesNamesQuery, IEnumerable<string>>
        {
            private readonly IStatelessSession _session;

            public UserCoursesNamesQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<string>> GetAsync(UserCoursesNamesQuery query, CancellationToken token)
            {
                return await _session.Query<UserCourse>()
                    .WithOptions(w => w.SetComment(nameof(UserCoursesNamesQuery)))
                    .Where(w => w.User.Id == query.UserId)
                    .Select(s => s.Course.Id)
                    .ToListAsync(token);
            }
        }
    }
}
