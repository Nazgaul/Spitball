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
    public class SuspendedUsersEmptyQuery : IQuery<IEnumerable<SuspendedUsersDto>>
    {
        internal sealed class SuspendedUsersEmptyQueryHandler : IQueryHandler<SuspendedUsersEmptyQuery, IEnumerable<SuspendedUsersDto>>
        {
            private readonly IStatelessSession _session;

            public SuspendedUsersEmptyQueryHandler(QuerySession session)
            {
                _session = session.StatelessSession;
            }

            public async Task<IEnumerable<SuspendedUsersDto>> GetAsync(SuspendedUsersEmptyQuery query, CancellationToken token)
            {
                var suspendDto = _session.Query<User>()
                    .Where(w => w.LockoutEnd != null)
                    .Select(s => new SuspendedUsersDto
                    {
                        UserId = s.Id,
                        UserEmail = s.Email,
                        LockoutEnd = s.LockoutEnd
                    });
                return await suspendDto.ToListAsync(cancellationToken: token);
            }
        }
    }
}
