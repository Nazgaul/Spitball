using System;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;

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

        public override async Task<User> LoadAsync(object id, CancellationToken token)
        {
            var user = await base.LoadAsync(id, token);
            CheckUserLockout(user);

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
    }
}