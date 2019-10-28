using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
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
            private readonly ICountryProvider _countryProvider;

            public UserBalanceDetailQueryHandler(QuerySession session, ICountryProvider countryProvider)
            {
                _countryProvider = countryProvider;
                _statelessSession = session.StatelessSession;
            }

            public async Task<IEnumerable<BalanceDto>> GetAsync(UserBalanceQuery query, CancellationToken token)
            {
                var listOfQueries = new Dictionary<TransactionType, IFutureValue<decimal?>>(); //List<IFutureValue<decimal>>();

                foreach (var value in Enum.GetValues(typeof(TransactionType)))
                {
                    var type = (TransactionType)value;
                    var xx = _statelessSession.Query<Transaction>()
                         .Where(w => w.User.Id == query.Id && w.Type == type)
                         .GroupBy(g => 1)
                         //.Select(s=>s.Key)
                         .Select(s => s.Sum(x => (decimal?)x.Price))
                         .ToFutureValue();
                    listOfQueries.Add(type, xx);
                }

                var futureValueCountry = _statelessSession.Query<User>().Where(w => w.Id == query.Id).Select(s => s.Country).ToFutureValue();


                var country = await futureValueCountry.GetValueAsync(token);


                var retVal = listOfQueries.Select(s =>
               {
                   var dbVal = s.Value.Value;
                   var value = _countryProvider.ConvertPointsToLocalCurrencyWithSymbol(country, dbVal.GetValueOrDefault());
                   return new BalanceDto(s.Key.ToString("G"), dbVal.GetValueOrDefault(), value);

               }).ToList();

                var totalBalance = retVal.Sum(x => x.Points);
                var totalValue = _countryProvider.ConvertPointsToLocalCurrencyWithSymbol(country, totalBalance);
                retVal.Add(new BalanceDto("Total", totalBalance, totalValue));

                return retVal;
            }
        }
    }
}