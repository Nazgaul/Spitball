using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserTransactionQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<TransactionDto>>
    {
        private readonly IStatelessSession _session;

        public UserTransactionQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<TransactionDto>> GetAsync(UserDataByIdQuery query, CancellationToken token)
        {
            var sqlQuery = _session.CreateSQLQuery(@"with cte
as
(
select id, Created, action,type,price 
from sb.[Transaction]
where user_id = :userId
) select Created as date,  action,type,price as amount,( select
SUM(price) as balance from cte  
 where id<=c.id) as balance
from cte c
order by id desc");
            sqlQuery.SetInt64("userId", query.Id);
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<TransactionDto>());

            return await sqlQuery.ListAsync<TransactionDto>(token);
        }
    }
}