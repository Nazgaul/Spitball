using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserAccountDataQueryHandler : IQueryHandler<UserDataByIdQuery, UserAccountDto>
    {
        private readonly ISession _session;

        public UserAccountDataQueryHandler(ReadonlySession readonlySession)
        {
            _session = readonlySession.Session;
        }

        public async Task<UserAccountDto> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var p = await _session.Query<User>()
                .Where(w => w.Id == query.Id).Select(s => new UserAccountDto()
                {
                    Id = s.Id,
                    Balance = s.Balance, // s.LastTransaction.Balance,
                    Name = s.Name,
                    Image = s.Image
                }).SingleOrDefaultAsync(cancellationToken: token).ConfigureAwait(false);

            return p;
        }
    }
}