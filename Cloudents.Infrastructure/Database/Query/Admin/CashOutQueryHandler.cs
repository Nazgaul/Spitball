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
using System;

namespace Cloudents.Infrastructure.Database.Query.Admin
{
    public class CashOutQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<CashOutDto>>
    {
        private readonly ISession _session;

        public CashOutQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            //TODO: remove the unessary stuff.
            //Use nhibernate future - note you need to use ISession

            var sqlQuery = _session.CreateSQLQuery(@"
                    select A.UserId as UserId
                            ,cast(count(distinct Q.UserId) as decimal) / count(distinct Q.id) as userQueryRatio
                    from sb.[Transaction] T1
                    inner join sb.Answer A
                            on A.UserId = T1.[User_id] and T1.[Action] = 'CashOut'
                    inner join sb.Question Q
                            on Q.CorrectAnswer_id = A.id
                    where T1.Created > DATEADD(WEEK, -2, getdate())
                    group by A.UserId, T1.Created
                    order by T1.Created desc"
            ).Future<dynamic>();

            TimeSpan twoWeeks = new TimeSpan(70, 0, 0, 0);

            var futureDto = _session.Query<Transaction>()
                .Fetch(f => f.User)
                .Where(w => w.Action == ActionType.CashOut)
                .Where(w => w.Created > DateTime.Now - twoWeeks)
                .Select(s => new
                {
                    UserId = s.User.Id,
                    s.User.Email,
                    s.Price,
                    s.Created,
                    s.User.FraudScore                   
                })
               .OrderByDescending(o => o.Created)
               .ToFuture<dynamic>();



            var z = await futureDto.GetEnumerableAsync();
            var t = sqlQuery.GetEnumerable();


            IEnumerable<CashOutDto> tempRes = z.Join(t, 
                                            u => u.UserId, 
                                            w => w[0], 
                                            (u,w) => new CashOutDto
                                            {
                                                UserId = u.UserId,
                                                UserEmail = u.Email,
                                                CashOutPrice = u.Price,
                                                CashOutTime = u.Created,
                                                FraudScore = Convert.ToInt32(u.FraudScore),
                                                UserQueryRatio = w[1]

                                            });
            
           return tempRes;

        }
    }
}
