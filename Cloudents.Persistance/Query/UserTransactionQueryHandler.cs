using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Application.DTOs;
using Cloudents.Application.Interfaces;
using Cloudents.Application.Query;
using Cloudents.Domain.Entities;
using NHibernate;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Database.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserTransactionQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<TransactionDto>>
    {
        private readonly IStatelessSession _session;
        private readonly QueryBuilder _queryBuilder;

        public UserTransactionQueryHandler(QuerySession session, QueryBuilder queryBuilder)
        {
            _queryBuilder = queryBuilder;
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<TransactionDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            
            var sqlQueryStr = $@"with cte
as
(
select {_queryBuilder.BuildProperty<Transaction>(x => x.Id)} as id, 
{_queryBuilder.BuildProperty<Transaction>(x => x.Created)} as Created,
{_queryBuilder.BuildProperty<Transaction>(x => x.Action)} as action,
{_queryBuilder.BuildProperty<Transaction>(x => x.Type)} as  type,
{_queryBuilder.BuildProperty<Transaction>(x => x.Price)} as price 
from {_queryBuilder.BuildTable<Transaction>()} 
where {_queryBuilder.BuildProperty<Transaction>(x => x.User)} = :userId
) select Created as {nameof(TransactionDto.Date)},
action as {nameof(TransactionDto.Action)},
type as {nameof(TransactionDto.Type)},
price as {nameof(TransactionDto.Amount)},
( select
SUM(price) as balance from cte  
 where id<=c.id) as {nameof(TransactionDto.Balance)}
from cte c
order by id desc";
            var sqlQuery = _session.CreateSQLQuery(sqlQueryStr);
            sqlQuery.SetInt64("userId", query.Id);
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<TransactionDto>());

            return await sqlQuery.ListAsync<TransactionDto>(token).ConfigureAwait(false);
        }
    }

    
}