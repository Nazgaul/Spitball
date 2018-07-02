using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using System.Linq;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserTransactionQueryHandler : IQueryHandler<UserDataByIdQuery,IEnumerable<TransactionDto>>
    {
        private readonly IStatelessSession _session;

        public UserTransactionQueryHandler(IStatelessSession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<TransactionDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var t = await _session.Query<Transaction>()
                .Where(w => w.User.Id == query.Id)
                .OrderBy(o => o.Id)
                .Select(s => new TransactionDto
                {
                    Balance = s.Balance,
                    Type = s.Type,
                    Amount = s.Price,
                    Action = s.Action,
                    Date = s.Created
                }).ToListAsync(cancellationToken: token).ConfigureAwait(false);
            return t;
        }
    }
}