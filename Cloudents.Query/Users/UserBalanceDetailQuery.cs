using Cloudents.Core.DTOs.Users;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Users
{
    public class UserBalanceQuery : IQuery<IEnumerable<BalanceDto>>
    {
        public UserBalanceQuery(long id)
        {
            Id = id;
        }

        private long Id { get; }




        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
        internal sealed class UserBalanceDetailQueryHandler : IQueryHandler<UserBalanceQuery, IEnumerable<BalanceDto>>
        {

            private readonly IStatelessSession _statelessSession;

            public UserBalanceDetailQueryHandler(QuerySession session)
            {
                _statelessSession = session.StatelessSession;
            }

            public async Task<IEnumerable<BalanceDto>> GetAsync(UserBalanceQuery query, CancellationToken token)
            {
                var listOfQueries = new Dictionary<TransactionType, IFutureValue<decimal?>>(); //List<IFutureValue<decimal>>();

                foreach (var value in Enum.GetValues(typeof(TransactionType)).Cast<TransactionType>())
                {
                    var type = value;
                    var xx = _statelessSession.Query<Transaction>()
                        .WithOptions(w => w.SetComment(nameof(UserBalanceQuery)))
                         .Where(w => w.User.Id == query.Id && w.Type == type)
                         .GroupBy(g => 1)
                         //.Select(s=>s.Key)
                         .Select(s => s.Sum(x => (decimal?)x.Price))
                         .ToFutureValue();
                    listOfQueries.Add(type, xx);
                }

                var futureValueCountry = _statelessSession.Query<User>().Where(w => w.Id == query.Id).Select(s => s.Country).ToFutureValue();


                Country country = await futureValueCountry.GetValueAsync(token);

                var retVal = listOfQueries.Select(s =>
                {
                    var dbVal = s.Value.Value;

                    var currencyValue = (country.ConversationRate * dbVal.GetValueOrDefault());
                    return new BalanceDto(s.Key.ToString("G"), dbVal.GetValueOrDefault(), currencyValue, country.RegionInfo.ISOCurrencySymbol);

                }).ToList();

                var totalBalance = retVal.Sum(x => x.Points);
                var totalCurrencyValue = (country.ConversationRate * totalBalance);
                retVal.Add(new BalanceDto("Total", totalBalance, totalCurrencyValue, country.RegionInfo.ISOCurrencySymbol));

                return retVal;
            }
        }
    }
}