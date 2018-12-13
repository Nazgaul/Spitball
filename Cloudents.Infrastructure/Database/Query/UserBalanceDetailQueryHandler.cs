using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Common.Enum;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using Cloudents.Infrastructure.Database.Repositories;
using NHibernate;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserBalanceDetailQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<BalanceDto>>
    {
        private readonly RegularUserRepository _userRepository;

        public UserBalanceDetailQueryHandler(/*ReadonlySession session,*/ RegularUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<BalanceDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var listOfQueries = new List<IFutureValue<decimal?>>();

            foreach (var value in Enum.GetValues(typeof(TransactionType)))
            {
                //query raise exception when one of the fields is null
                //TODO check defaultIfEmpty

                var xx = _userRepository.UserBalanceByType(query.Id, (TransactionType) value)
                    .FutureValue<decimal?>();
                listOfQueries.Add(xx);
            }

            await listOfQueries[0].GetValueAsync(token).ConfigureAwait(false);

            var decimals = listOfQueries.Select(s => s.Value);
            return decimals.Select(
                (s, i) => new BalanceDto((TransactionType)i, s.GetValueOrDefault()));
        }
    }
}