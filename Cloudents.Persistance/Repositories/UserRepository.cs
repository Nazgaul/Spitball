using System;
using System.Collections.Generic;
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
    public class RegularUserRepository : NHibernateRepository<User>, IRegularUserRepository
    {
        public RegularUserRepository(ISession session) : base(session)
        {
        }


       

        public Task<decimal> UserBalanceAsync(long userId, CancellationToken token)
        {
            return UserAvailableBalance(userId)
                .SingleOrDefaultAsync<decimal>(token);
        }

        public  Task<User> GetUserByEmailAsync(string userEmail, CancellationToken token)
        {
            return
                Session.QueryOver<User>()
                    .Where(w => w.Email == userEmail)
                    .SingleOrDefaultAsync(token);
        }

        public async Task<IEnumerable<User>> GetExpiredCreditCardsAsync(CancellationToken token)
        {
            return await Session.QueryOver<User>()
                .Where(w => w.BuyerPayment.PaymentKeyExpiration < DateTime.UtcNow)
                .ListAsync<User>(token);

        }

        /*  public Task<decimal> UserBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.Price)).SingleOrDefaultAsync<decimal>(token);
        }*/

        //internal IQueryOver<Transaction, Transaction> UserBalanceByType(long userId, TransactionType type)
        //{

        //    return
        //      Session.QueryOver<Transaction>()
        //          .Where(w => w.User.Id == userId)
        //          .Where(w => w.Type == type)
        //          .Select(Projections.Sum<Transaction>(x => x.Price));
        //}

        private IQueryOver<Transaction, Transaction> UserAvailableBalance(long userId)
        {
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == TransactionType.Earned || w.Type == TransactionType.Stake)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }
       
    }
}