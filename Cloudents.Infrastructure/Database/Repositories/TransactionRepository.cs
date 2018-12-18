using Cloudents.Core.Interfaces;
using Cloudents.Domain.Entities;
using JetBrains.Annotations;
using System.Threading.Tasks;
using System.Threading;
using Cloudents.Common.Enum;
using NHibernate;
using NHibernate.Criterion;
using Cloudents.Core.Event;

namespace Cloudents.Infrastructure.Database.Repositories
{
    [UsedImplicitly]
    public class TransactionRepository : NHibernateRepository<Transaction>, ITransactionRepository
    {
        //TODO: not sure if its ok
        private readonly IEventStore _store;
        public TransactionRepository(ISession session, IEventStore store) : base(session)
        {
            _store = store;
        }

        public override Task<object> AddAsync(Transaction entity, CancellationToken token)
        {
            _store.Add(new TransactionEvent(entity));
            return base.AddAsync(entity, token);
        }

        public Task<decimal> GetBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<decimal>(token);
        }
        public Task<int> GetUserScoreAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                       .Where(w => w.User.Id == userId)
                       .Where(w => w.Type == TransactionType.Earned && w.Price > 0)
                       .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<int>(token);
        }
    }
}
