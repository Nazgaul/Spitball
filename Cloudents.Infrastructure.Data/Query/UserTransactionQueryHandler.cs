using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using System.Linq;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Data.Query
{
    public class UserTransactionQueryHandler : IQueryHandler<UserDataByIdQuery, IEnumerable<TransactionDto>>
    {
        private readonly IStatelessSession _session;

        public UserTransactionQueryHandler(IStatelessSession session)
        {
            _session = session;
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
from cte c");
            sqlQuery.SetInt64("userId", query.Id);
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<TransactionDto>());

            var t = await sqlQuery.ListAsync<TransactionDto>();
            return t;

            /**/
            //var t = await _session.Query<Transaction>()
            //    .Where(w => w.User.Id == query.Id)
            //    .OrderBy(o => o.Id)
            //    .Select(s => new TransactionDto
            //    {
            //        //Balance = s.Balance,
            //        Type = s.Type,
            //        Amount = s.Price,
            //        Action = s.Action,
            //        Date = s.Created
            //    }).ToListAsync(cancellationToken: token).ConfigureAwait(false);
            //return t;
        }
    }
}