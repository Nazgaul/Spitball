using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Persistence.Repositories
{

    public class FictiveUserRepository : NHibernateRepository<SystemUser>, IFictiveUserRepository
    {
        public FictiveUserRepository(ISession session) : base(session)
        {
        }
        public Task<SystemUser> GetRandomFictiveUserAsync(string country, CancellationToken token)
        {
            return Session.QueryOver<SystemUser>().Where(w => w.Country == country)
                .OrderByRandom()
                .Take(1)
                .SingleOrDefaultAsync<SystemUser>(token);
        }
    }

    [UsedImplicitly]
    public class RegularUserRepository : NHibernateRepository<RegularUser>, IRegularUserRepository
    {
        public RegularUserRepository(ISession session) : base(session)
        {
        }


        //public override async Task<RegularUser> GetAsync(object id, CancellationToken token)
        //{
        //    var user = await base.GetAsync(id, token);
        //    CheckUserLockout(user);

        //    return user;
        //}

        //public override RegularUser Load(object id)
        //{
        //    var user = base.Load(id);
        //    CheckUserLockout(user);

        //    return user;
        //}

        //public override Task<RegularUser> LoadAsync(object id, CancellationToken token)
        //{
        //    return LoadAsync(id, true, token);
        //}

        //public async Task<RegularUser> LoadAsync(object id, bool checkUserLocked, CancellationToken token)
        //{
        //    var user = await base.LoadAsync(id, token);
        //    if (checkUserLocked)
        //    {
        //        CheckUserLockout(user);
        //    }

        //    return user;
        //}

        //private static void CheckUserLockout([NotNull] RegularUser user)
        //{
        //    if (user == null) throw new ArgumentNullException(nameof(user));
        //    if (!user.LockoutEnabled)
        //    {
        //        return;
        //    }
        //    if (user.LockoutEnd.HasValue && DateTime.UtcNow < user.LockoutEnd.Value)
        //    {
        //        throw new UserLockoutException();
        //    }
        //}

        public Task<decimal> UserCashableBalanceAsync(long userId, CancellationToken token)
        {
            return UserAvailableBalance(userId)
                .SingleOrDefaultAsync<decimal>(token);
        }

       

      /*  public Task<decimal> UserBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<decimal>(token);
        }*/

        internal IQueryOver<Transaction, Transaction> UserBalanceByType(long userId, TransactionType type)
        {
   
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == type)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }

        internal IQueryOver<Transaction, Transaction> UserAvailableBalance(long userId)
        {
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == TransactionType.Earned || w.Type == TransactionType.Stake)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }

       /* public Task UpdateUsersBalance(CancellationToken token)
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

        }*/
    }
}