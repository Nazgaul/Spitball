using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace Cloudents.Persistence.Repositories
{

    //public class FictiveUserRepository : NHibernateRepository<SystemUser>, IFictiveUserRepository
    //{
    //    public FictiveUserRepository(ISession session) : base(session)
    //    {
    //    }
    //    public Task<SystemUser> GetRandomFictiveUserAsync(string country, CancellationToken token)
    //    {
    //        return Session.QueryOver<SystemUser>().Where(w => w.Country == country)
    //            .OrderByRandom()
    //            .Take(1)
    //            .SingleOrDefaultAsync<SystemUser>(token);
    //    }
    //}

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

        public Task<User> GetUserByEmailAsync(string userEmail, CancellationToken token)
        {
            return
                Session.QueryOver<User>()
                    .Where(w => w.Email == userEmail)
                    .SingleOrDefaultAsync(token);
        }

        public async Task<IEnumerable<User>> GetExpiredCreditCardsAsync(CancellationToken token)
        {
            return await Session.QueryOver<PaymePayment>()
                .Where(w => w.PaymentKeyExpiration < DateTime.UtcNow)
                .ListAsync<User>(token);

        }

        private IQueryOver<Transaction, Transaction> UserAvailableBalance(long userId)
        {
            return
              Session.QueryOver<Transaction>()
                  .Where(w => w.User.Id == userId)
                  .Where(w => w.Type == TransactionType.Earned)
                  .Select(Projections.Sum<Transaction>(x => x.Price));
        }


        public override async Task DeleteAsync(User entity, CancellationToken token)
        {
            var documents = await entity.Documents.AsQueryable().Select(s => s.Id).ToListAsync(cancellationToken: token);

            if (documents.Count > 0)
            {
                var sqlQuery =
                    Session.CreateSQLQuery("delete from sb.DocumentsTags where documentid in (:Id)");
                sqlQuery.SetParameterList("Id", documents);

                await sqlQuery.ExecuteUpdateAsync(token);
            }

            await Session.CreateSQLQuery("delete from sb.Answer where userId = :Id")
                .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);


            await Session.CreateSQLQuery("delete from sb.Answer where questionId in (select Id from sb.question where userId = :Id)")
                .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);

            await Session.CreateSQLQuery("delete from sb.question where userId = :Id")
                .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);

         
        
            await Session.CreateSQLQuery("delete from sb.UsersCourses where userId = :Id")
                    .SetInt64("Id", entity.Id).ExecuteUpdateAsync(token);
        

            var sqlQuery2 =
                Session.CreateSQLQuery("delete from sb.UsersTags where userId = :Id");
            sqlQuery2.SetInt64("Id", entity.Id);
            await sqlQuery2.ExecuteUpdateAsync(token);


            var sqlQuery3 =
                Session.CreateSQLQuery("delete from sb.UserExtension where userId = :Id");
            sqlQuery3.SetInt64("Id", entity.Id);
            await sqlQuery3.ExecuteUpdateAsync(token);

            await base.DeleteAsync(entity, token);
        }
    }
}