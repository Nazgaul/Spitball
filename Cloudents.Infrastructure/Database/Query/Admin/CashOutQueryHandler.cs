using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    public class CashOutQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<CashOutDto>>
    {
        private readonly IStatelessSession _session;

        public CashOutQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            //TODO: remove the unessary stuff.
            //Use nhibernate future - note you need to use ISession
            var sqlQuery = _session.CreateSQLQuery(@"
                    select U.id as UserId, T1.Price as CashOutPrice, T1.Created as CashOutTime
	                    ,case when avg(DATEDIFF(SECOND, Q.Created, A.Created)) < 80 
			                    or avg(DATEDIFF(SECOND, A.Created, T.Created)) < 400 
			                    or cast(count(distinct Q.UserId) as decimal) / count(distinct Q.id) < 0.1 then cast(1 as bit)
			                    else cast(0 as bit) end as IsSuspect
                    from sb.[Transaction] T1
                    inner join sb.[User] U
	                    on U.Id = T1.[User_id] and T1.[Action] = 'CashOut'
                    inner join sb.Answer A
	                    on U.id = A.UserId
                    inner join sb.Question Q
	                    on Q.CorrectAnswer_id = A.id
                    inner join sb.[Transaction] T
	                    on T.[Action] = 'AnswerCorrect'
	                    and T.AnswerId = A.id
                    group by U.id, T1.Price, T1.Created
                    order by T1.Created desc"
                );

            //_session.Query<Transaction>()
            //    .Fetch(f=>f.User)
            //    .Where(w=>w.Action == ActionType.CashOut)
            //    .Select(s=> new
            //    {
            //        s.User.Email,
                    
            //    })
            //_session.Query<User>().Fetch(f=>f.Transactions).Where(w=>w.Transactions.)
            
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<CashOutDto>());

            return await sqlQuery.ListAsync<CashOutDto>(token).ConfigureAwait(false);

        }
    }
}
