using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Infrastructure.Database.Repositories
{
    [UsedImplicitly]
    public class UserRepository : NHibernateRepository<User>, IUserRepository
    {
        public UserRepository(ISession session) : base(session)
        {
        }


        public override async Task<User> GetAsync(object id, CancellationToken token)
        {
            var user = await base.GetAsync(id, token);
            CheckUserLockout(user);

            return user;
        }

        public override User Load(object id)
        {
            var user = base.Load(id);
            CheckUserLockout(user);

            return user;
        }

        public override Task<User> LoadAsync(object id, CancellationToken token)
        {
            return LoadAsync(id, true, token);
            //var user = await base.LoadAsync(id, token);
            //CheckUserLockout(user);

            //return user;
        }

        public async Task<User> LoadAsync(object id, bool checkUserLocked, CancellationToken token)
        {
            var user = await base.LoadAsync(id, token);
            if (checkUserLocked)
            {
                CheckUserLockout(user);
            }

            return user;
        }

        private static void CheckUserLockout([NotNull] User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (user.LockoutEnd.HasValue && DateTime.UtcNow < user.LockoutEnd.Value)
            {
                throw new UserLockoutException();
            }
        }

        public Task<decimal> UserEarnedBalanceAsync(long userId, CancellationToken token)
        {
            return UserBalanceByType(userId, TransactionType.Earned)
                .SingleOrDefaultAsync<decimal>(token);
        }

        public Task<User> GetRandomFictiveUserAsync(CancellationToken token)
        {
            return Session.QueryOver<User>().Where(w => w.Fictive)
                   .OrderByRandom()
                   .Take(1)
                   .SingleOrDefaultAsync<User>(token);
        }

        public Task<decimal> UserBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<decimal>(token);
        }

        internal IQueryOver<Transaction, Transaction> UserBalanceByType(long userId, TransactionType type)
        {
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == type)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }

        public Task UpdateBalance(long id, CancellationToken token)
        {
            var test = Session.Query<Transaction>()
                .Where(w => w.User.Id == id)
                .Select(s => s.Price);
            var sum = test.Aggregate((a, b) => a + b);

            return Session.Query<User>().Where(w => w.Id == id).Where(w => w.Balance != sum)
                  .UpdateBuilder()
                  .Set(x => x.Balance, x => sum)
                  .UpdateAsync(token);

        }
    }
}