using Cloudents.Core.Interfaces;
using Cloudents.Core.Entities.Db;
using JetBrains.Annotations;
using System.Threading.Tasks;
using System.Threading;
using NHibernate;
using NHibernate.Criterion;
using System.Collections.Generic;

namespace Cloudents.Infrastructure.Database.Repositories
{
    [UsedImplicitly]
    public class TransactionRepository : NHibernateRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(ISession session) : base(session)
        {
        }

        public Task<decimal> GetBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<decimal>(token);
        }
    }
}
