﻿using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using JetBrains.Annotations;
using NHibernate;
using NHibernate.Criterion;

namespace Cloudents.Persistance.Repositories
{
    [UsedImplicitly]
    public class TransactionRepository : NHibernateRepository<Transaction>, ITransactionRepository
    {
        //TODO: not sure if its ok
        //private readonly IEventStore _store;
        public TransactionRepository(ISession session) : base(session)
        {
        }

        public override Task<object> AddAsync(Transaction entity, CancellationToken token)
        {
            return base.AddAsync(entity, token);
        }

        public Task<decimal> GetBalanceAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == userId)
                    .Select(Projections.Sum<Transaction>(x => x.TransactionType.Price))
                    .SingleOrDefaultAsync<decimal>(token);
        }
        public Task<decimal> GetUserScoreAsync(long userId, CancellationToken token)
        {
            return
                Session.QueryOver<Transaction>()
                       .Where(w => w.User.Id == userId)
                       .Where(w => w.TransactionType.Type == TransactionType.Earned && w.TransactionType.Price > 0)
                       .Select(Projections.Sum<Transaction>(x => x.TransactionType.Price)).SingleOrDefaultAsync<decimal>(token);
        }

        public async Task<TransactionActionType> GetFirstCourseTransaction (long userId, CancellationToken token)
        {
            return await Session.QueryOver<Transaction>()
                .Where(w => w.User.Id == userId)
                .Where(w => w.TransactionType.Action == TransactionActionType.FirstCourse)
                .Select(s => s.TransactionType.Action).SingleOrDefaultAsync<TransactionActionType>(token);

                //user.Transactions.Where(w => w.Action == TransactionActionType.FirstCourse)
               //                                             .Select(s => s.Action).FirstOrDefault();
        }
    }
}
