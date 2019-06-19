using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class SuspendedUsersQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<SuspendedUsersDto>>
    {
        private readonly IStatelessSession _session;

        public SuspendedUsersQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<SuspendedUsersDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
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
