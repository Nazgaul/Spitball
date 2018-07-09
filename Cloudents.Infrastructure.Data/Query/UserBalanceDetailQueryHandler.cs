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
using Cloudents.Infrastructure.Data.Repositories;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserBalanceDetailQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<BalanceDto>>
    {
        private readonly ISession _session;
        private readonly UserRepository _userRepository;

        public UserBalanceDetailQueryHandler(ReadonlySession session, UserRepository userRepository)
        {
            _userRepository = userRepository;
            _session = session.Session;
        }

        public async Task<IEnumerable<BalanceDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var l = new List<IFutureValue<decimal?>>();

            // // Pending,
            // Stake,
            foreach (var value in Enum.GetValues(typeof(TransactionType)))
            {
                //query raise exception when one of the fields is null
                //TODO check defaultIfEmpty

                var xx = _userRepository.UserBalanceByType(query.Id, (TransactionType) value)
                    .FutureValue<decimal?>();
                l.Add(xx);
            }

            //Question questionAlias = null;
            //var pendingFuture = _session.QueryOver<Answer>()
            //      .Where(w => w.User.Id == query.Id)
            //      //.JoinQueryOver(j => j.Question)
            //      .JoinAlias(x => x.Question, () => questionAlias)
            //      .Where(() => questionAlias.CorrectAnswer == null)
            //      .Select(Projections.Sum(() => questionAlias.Price))
            //      .FutureValue<decimal?>();

            //var stakeFuture = _session.QueryOver<Question>()
            //    .Where(w => w.User.Id == query.Id)
            //    .And(w => w.CorrectAnswer == null)
            //    .Select(Projections.Sum<Question>(x => x.Price))
            //    .FutureValue<decimal?>();

            await l[0].GetValueAsync(token).ConfigureAwait(false);

            // var pending = new BalanceDto("Pending", pendingFuture.Value.GetValueOrDefault());
            //var stake = new BalanceDto("Stake", stakeFuture.Value.GetValueOrDefault());

            var decimals = l.Select(s => s.Value);
            return decimals.Select(
                (s, i) => new BalanceDto((TransactionType)i, s.GetValueOrDefault()))/*.Union(new[] { stake, pending })*/;
        }
    }
}