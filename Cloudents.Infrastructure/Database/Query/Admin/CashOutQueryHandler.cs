﻿using System.Collections.Generic;
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

        class fristQuery
        {
            public long userId { get; set; }
            public decimal userQueryRatio { get; set; }
        }

        public class secoundQuery
        {
            public long userId { get; set; }
            public string email { get; set; }
            public decimal price { get; set; }
            public DateTime created { get; set; }
            public int? fraudScore { get; set; }
        }

        public CashOutQueryHandler(ReadonlySession session)
        {
            _session = session.Session;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            //TODO: remove the unessary stuff.
            //Use nhibernate future - note you need to use ISession
            TimeSpan twoWeeks = new TimeSpan(14, 0, 0, 0);
          
            var sqlQuery = _session.CreateSQLQuery($@"select A.UserId as UserId
                                                        ,cast(count(distinct Q.UserId) as decimal) / count(distinct Q.id) as userQueryRatio 
                                                    from sb.[Transaction] T1 
                                                    inner join sb.Answer A 
                                                        on A.UserId = T1.[User_id] and T1.[Action] = 'CashOut' 
                                                    inner join sb.Question Q 
                                                        on Q.CorrectAnswer_id = A.id 
                                                    where T1.Created > DATEADD(WEEK, -2, getdate()) 
                                                    group by A.UserId, T1.Created 
                                                    order by T1.Created desc"
                                                    )
            .SetResultTransformer(Transformers.AliasToBean<fristQuery>())
            .Future<fristQuery>();

           

            var futureDto = _session.Query<Transaction>()
                .Fetch(f => f.User)
                .Where(w => w.Action == ActionType.CashOut)
                .Where(w => w.Created > DateTime.Now - twoWeeks)
                .Select(s => new secoundQuery
                {
                    userId = s.User.Id,
                    email = s.User.Email,
                    price = s.Price,
                    created = s.Created,
                    fraudScore = s.User.FraudScore
                })

               .OrderByDescending(o => o.created)

               .ToFuture();



            var z = await futureDto.GetEnumerableAsync();
            var t = sqlQuery.GetEnumerable();
            var tempRes = new List<CashOutDto>();

            for (int i = 0; i < t.Count(); i++)
            {
                try
                {
                    var index = z.ElementAt(i);
                    var tindex = t.ElementAt(i);
                
                tempRes.Add(new CashOutDto
                {

                    UserId = index.userId,
                    UserEmail = index.email,
                    CashOutPrice = index.price,
                    CashOutTime = index.created,
                    FraudScore = index.fraudScore,
                    userQueryRatio = tindex.userQueryRatio

                });
                }
                catch
                {
                    throw;
                }

            }

      

            return tempRes;

        }
    }
}
