using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    //public class TransactionRepository : NHibernateRepository<Transaction>, ITransactionRepository
    //{
    //    public TransactionRepository(ISession session) : base(session)
    //    {
    //    }

    //    public async Task<decimal> GetCurrentBalanceAsync(long userId, CancellationToken token)
    //    {
    //        return await Session.Query<Transaction>()
    //            .Where(w => w.User.Id == userId)
    //            .SumAsync(s => s.Price, cancellationToken: token).ConfigureAwait(false);
    //    }
    //}
}