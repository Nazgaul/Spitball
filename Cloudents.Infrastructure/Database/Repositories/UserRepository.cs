using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Exceptions;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;
using System;
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
            if (user.Fictive.GetValueOrDefault())
            {
                return;
            }
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

        public Task<User> GetRandomFictiveUserAsync(string country, CancellationToken token)
        {
            return Session.QueryOver<User>().Where(w => w.Fictive == true && w.Country == country)
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

        public Task UpdateUsersBalance(CancellationToken token)
        {
            //TODO: need to make this query using Linq instead of sql
            var updateQuery = Session.CreateSQLQuery(
                @"update sb.[user] 
                set balance = (Select sum(price) 
                                from sb.[Transaction] 
                                where User_id = sb.[User].id
                                )
                where balance != (Select sum(price) 
                                    from sb.[Transaction] 
                                    where User_id = sb.[User].id
                                )");
            return updateQuery.ExecuteUpdateAsync(token);

        }
    }
}