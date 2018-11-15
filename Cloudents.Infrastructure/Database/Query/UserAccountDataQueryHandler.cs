using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Query
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
            //TODO check this query
            return await _session.Query<User>()
                .Where(w => w.Id == query.Id).Select(s => new UserAccountDto
                {
                    Id = s.Id,
                    Balance = s.Balance, // s.LastTransaction.Balance,
                    Name = s.Name,
                    Image = s.Image,
                    Email = s.Email,
                    UniversityExists = s.University != null
                    //UniversityId = s.University.Id
                }).WithOptions(o =>
                {
                    o.SetCacheable(true)
                        .SetReadOnly(true);
                }).SingleOrDefaultAsync(token).ConfigureAwait(false);
        }
    }
}