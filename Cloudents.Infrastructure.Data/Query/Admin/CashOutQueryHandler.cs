using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query.Admin;
using NHibernate;
using NHibernate.Transform;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;



namespace Cloudents.Infrastructure.Data.Query.Admin
{
    public class CashOutQueryHandler : IQueryHandler<CashOutQuery, IEnumerable<CashOutDto>>
    {
        private readonly IStatelessSession _session;

        public CashOutQueryHandler(ReadonlyStatelessSession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(CashOutQuery query, CancellationToken token)
        {

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
                    order by T1.Created"
                );
            
            sqlQuery.SetResultTransformer(Transformers.AliasToBean<CashOutDto>());

            return await sqlQuery.ListAsync<CashOutDto>(token).ConfigureAwait(false);

        }
    }
}
