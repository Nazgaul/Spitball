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

        public async Task<IEnumerable<BalanceDto>> GetCurrentBalanceDetailAsync(long userId, CancellationToken token)
        {
            var l = new List<IFutureValue<decimal?>>();
            foreach (var value in Enum.GetValues(typeof(TransactionType)))
            {
                //query raise exception when one of the fields is null
                //TODO check defaultIfEmpty

                var xx = Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Where(w => w.Type == (TransactionType) value)
                    .Select(Projections.Sum<Transaction>(x => x.Price))
                    .FutureValue<decimal?>();
                l.Add(xx);
            }

            await l[0].GetValueAsync(token).ConfigureAwait(false);
            var decimals = l.Select(s => s.Value);
            return decimals.Select((s, i) => new BalanceDto((TransactionType) i, s.GetValueOrDefault()));
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