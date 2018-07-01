using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserBalanceDetailQueryHandler : IQueryHandlerAsync<UserBalanceQuery, IEnumerable<BalanceDto>>
    {
        private readonly ISession _session;

        public UserBalanceDetailQueryHandler(ISession session)
        {
            _session = session;
        }

        public async Task<IEnumerable<BalanceDto>> GetAsync(UserBalanceQuery query, CancellationToken token)
        {
            var l = new List<IFutureValue<decimal?>>();

            // // Pending,
            // Stake,
            foreach (var value in Enum.GetValues(typeof(TransactionType)))
            {
                //query raise exception when one of the fields is null
                //TODO check defaultIfEmpty

                var xx = _session.QueryOver<Transaction>()
                    .Where(w => w.User.Id == query.UserId)
                    .Where(w => w.Type == (TransactionType)value)
                    .Select(Projections.Sum<Transaction>(x => x.Price))
                    .FutureValue<decimal?>();
                l.Add(xx);
            }

            Question questionAlias = null;
            var pendingFuture = _session.QueryOver<Answer>()
                  .Where(w => w.User.Id == query.UserId)
                  //.JoinQueryOver(j => j.Question)
                  .JoinAlias(x => x.Question, () => questionAlias)
                  .Where(() => questionAlias.CorrectAnswer == null)
                  .Select(Projections.Sum(() => questionAlias.Price))
                  .FutureValue<decimal?>();
            //.JoinAlias(x => x.Question, () => questionAlias)
            //.JoinQueryOver(w=>w.Question)
            //.Fetch(f => f.Question)

            var stakeFuture = _session.QueryOver<Question>()
                .Where(w => w.User.Id == query.UserId)
                .And(w => w.CorrectAnswer == null)
                .Select(Projections.Sum<Question>(x => x.Price))
                .FutureValue<decimal?>();

            //.Where(w => w.Question.CorrectAnswer != null)



            await l[0].GetValueAsync(token).ConfigureAwait(false);

             var pending = new BalanceDto("Pending", pendingFuture.Value.GetValueOrDefault());
            var stake = new BalanceDto("Stake", stakeFuture.Value.GetValueOrDefault());


            var decimals = l.Select(s => s.Value);
            return decimals.Select(
                (s, i) => new BalanceDto((TransactionType)i, s.GetValueOrDefault())).Union(new[] { stake, pending });
        }
    }
}