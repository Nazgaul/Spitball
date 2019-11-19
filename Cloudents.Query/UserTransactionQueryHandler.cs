using Cloudents.Core.DTOs;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class UserTransactionQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<TransactionDto>>
    {
        private readonly IStatelessSession _session;

        public UserTransactionQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<TransactionDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {

            var sqlQueryStr = $@"with cte
as
(
select id as id, 
created as Created,
action as action,
type as  type,
price as price 
from sb.[transaction]
where user_id = :userId
) select Created as Date,
action as Action,
type as Type,
price as Amount,
( select
SUM(price) as balance from cte  
 where id<=c.id) as Balance
from cte c
order by id desc";
            var sqlQuery = _session.CreateSQLQuery(sqlQueryStr);
            sqlQuery.SetInt64("userId", query.Id);
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<TransactionDto>());

            return await sqlQuery.ListAsync<TransactionDto>(token);
        }
    }


}