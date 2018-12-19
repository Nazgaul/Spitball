using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs.Admin;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query.Admin;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class SuspendedUsersQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<SuspendedUsersDto>>
    {
        private readonly ISession _session;

        public SuspendedUsersQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<SuspendedUsersDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            var suspendDto = _session.Query<RegularUser>()
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
