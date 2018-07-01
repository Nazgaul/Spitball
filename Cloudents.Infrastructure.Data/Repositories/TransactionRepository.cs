using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class TransactionRepository : NHibernateRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ISession session) : base(session)
        {
        }

       

        public async Task<IEnumerable<TransactionDto>> GetTransactionsAsync(long userId, CancellationToken token)
        {
            var t = await Session.Query<Transaction>()
                .Where(w => w.User.Id == userId)
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