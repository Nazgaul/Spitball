using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [UsedImplicitly]
    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        public UserRepository(ISession session) : base(session)
        {
        }

        public async Task<IList<User>> GetAllUsersAsync(CancellationToken token)
        {
            var t = await Session.Query<User>().Where(w => w.Email != null).ToListAsync(token);
            return t;
        }

        public Task<decimal> UserEarnedBalanceAsync(long userId, CancellationToken token)
        {
            return UserBalanceByType(userId, TransactionType.Earned)
                .SingleOrDefaultAsync<decimal>(token);
        }

        internal IQueryOver<Transaction, Transaction> UserBalanceByType(long userId, TransactionType type)
        {
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == type)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }


    }
}