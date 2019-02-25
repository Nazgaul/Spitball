using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Query.Query;
using NHibernate;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserBalanceDetailQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<BalanceDto>>
    {
       // private readonly RegularUserRepository _userRepository;

       private readonly IStatelessSession _statelessSession;

        public UserBalanceDetailQueryHandler(QuerySession session)
        {
            _statelessSession = session.StatelessSession;
        }

        public async Task<IEnumerable<BalanceDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var listOfQueries = new List<IFutureValue<decimal?>>();

            foreach (var value in Enum.GetValues(typeof(TransactionType)))
            {
                //query raise exception when one of the fields is null
                //TODO check defaultIfEmpty
                var sqlQuery =
                    _statelessSession.CreateSQLQuery(
                        "select sum(price) from sb.[Transaction] where User_id = :userId and Type = :type");
                sqlQuery.SetInt64("userId", query.Id);
                sqlQuery.SetString("type", value.ToString());

                var xx = sqlQuery.FutureValue<decimal?>();
                listOfQueries.Add(xx);
            }

            await listOfQueries[0].GetValueAsync(token);

            var decimals = listOfQueries.Select(s => s.Value);
            return decimals.Select(
                (s, i) => new BalanceDto((TransactionType)i, s.GetValueOrDefault()));
        }
    }
}