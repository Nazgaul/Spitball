using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class SuspendedUsersQuery : IQueryAdmin<IEnumerable<SuspendedUsersDto>>
    {
        public SuspendedUsersQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }
        internal sealed class SuspendedUsersEmptyQueryHandler : IQueryHandler<SuspendedUsersQuery, IEnumerable<SuspendedUsersDto>>
        {
            private readonly IStatelessSession _session;

            public SuspendedUsersEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<SuspendedUsersDto>> GetAsync(SuspendedUsersQuery query, CancellationToken token)
            {
                var suspendDto = _session.Query<User>()
                    .Where(w => w.LockoutEnd != null);
                if (!string.IsNullOrEmpty(query.Country))
                {
                    suspendDto = suspendDto.Where(w => w.Country == query.Country);
                }

                var res = suspendDto.Select(s => new SuspendedUsersDto
                {
                    UserId = s.Id,
                    UserEmail = s.Email,
                    LockoutEnd = s.LockoutEnd
                });
                return await res.ToListAsync(cancellationToken: token);
            }
        }
    }
}
