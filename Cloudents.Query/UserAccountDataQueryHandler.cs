using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserAccountDataQueryHandler : IQueryHandler<UserDataByIdQuery, UserAccountDto>
    {
        private readonly IStatelessSession _session;

        public UserAccountDataQueryHandler(QuerySession readonlySession)
        {
            _session = readonlySession.StatelessSession;
        }

        //[Cache(TimeConst.Minute * 15, "UserAccount", true)]
        public async Task<UserAccountDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            return await _session.Query<RegularUser>()
                .Where(w => w.Id == query.Id && (!w.LockoutEnd.HasValue || DateTime.UtcNow >= w.LockoutEnd.Value))
                .Select(s => new UserAccountDto
                {
                    Id = s.Id,
                    Balance = s.Transactions.Balance, 
                    Name = s.Name,
                    Image = s.Image,
                    Email = s.Email,
                    UniversityExists = s.University.Id != null,
                    Score = s.Transactions.Score,
                    PhoneNumber = s.PhoneNumber
                }).WithOptions(o =>
                {
                    o.SetCacheable(true)
                        .SetReadOnly(true);
                }).SingleOrDefaultAsync(token);
        }
    }
}