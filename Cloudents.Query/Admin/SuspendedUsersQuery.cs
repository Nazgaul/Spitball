﻿using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class SuspendedUsersQuery : IQueryAdmin2<IEnumerable<SuspendedUsersDto>>
    {
        public SuspendedUsersQuery(Country? country)
        {
            Country = country;
        }
        public Country? Country { get; }
        internal sealed class SuspendedUsersEmptyQueryHandler : IQueryHandler<SuspendedUsersQuery, IEnumerable<SuspendedUsersDto>>
        {
            private readonly IStatelessSession _session;

            public SuspendedUsersEmptyQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<SuspendedUsersDto>> GetAsync(SuspendedUsersQuery query, CancellationToken token)
            {
                var suspendDto = _session.Query<User>()
                    .WithOptions(w => w.SetComment(nameof(SuspendedUsersQuery)))
                    .Where(w => w.LockoutEnd != null);
                if (query.Country != null)
                {
                    suspendDto = suspendDto.Where(w => w.SbCountry == query.Country);
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
