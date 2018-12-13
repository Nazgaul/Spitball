﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Domain.Entities;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
