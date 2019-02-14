using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Cloudents.Query.Query.Admin;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Transform;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class CashOutQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<CashOutDto>>
    {
        private readonly IStatelessSession _session;

        private class FirstQuery
        {
            public long UserId { get; set; }
            public decimal UserQueryRatio { get; set; }
        }

        private class SecondQuery
        {
            public long UserId { get; set; }
            public string Email { get; set; }
            public decimal Price { get; set; }
            public DateTime Created { get; set; }
            public int? FraudScore { get; set; }
            public bool IsIsrael { get; set; }
            public bool? Approved { get; set; }
            public string DeclinedReason { get; set; }
            public Guid TransactionId { get; set; }
        }

        public CashOutQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {

            var twoWeeks = TimeSpan.FromDays(14);

            var sqlQuery = _session.CreateSQLQuery(@"select A.UserId as UserId
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
            .SetResultTransformer(Transformers.AliasToBean<FirstQuery>())
            .Future<FirstQuery>();

            var futureDto = _session.Query<CashOutTransaction>()
                .Fetch(f => f.User)
                .Where(w => w.Action == TransactionActionType.CashOut)
                .Where(w => w.Created > DateTime.Now - twoWeeks)
                .Select(s => new SecondQuery
                {
                    UserId = s.User.Id,
                    Email = s.User.Email,
                    Price = s.Price,
                    Created = s.Created,
                    FraudScore = s.User.FraudScore,
                    IsIsrael = s.User.Country == "IL",
                    Approved = s.Approved,
                    DeclinedReason = s.DeclinedReason,
                    TransactionId = s.Id

                })

               .OrderByDescending(o => o.Created)
               .ToFuture();



            var z = (await futureDto.GetEnumerableAsync(token)).ToArray();
            var t = sqlQuery.GetEnumerable().ToArray();
            var tempRes = new List<CashOutDto>();

            for (var i = 0; i < t.Length; i++)
            {
                var index = z.ElementAt(i);
                var tIndex = t.ElementAt(i);

                tempRes.Add(new CashOutDto
                {

                    UserId = index.UserId,
                    UserEmail = index.Email,
                    CashOutPrice = index.Price,
                    CashOutTime = index.Created,
                    FraudScore = index.FraudScore,
                    UserQueryRatio = tIndex.UserQueryRatio,
                    IsIsrael = index.IsIsrael,
                    Approved = index.Approved,
                    DeclinedReason = index.DeclinedReason,
                    TransactionId = index.TransactionId
                });

            }
            return tempRes;
        }
    }
}
