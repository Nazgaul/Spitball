using Cloudents.Core.DTOs.Admin;
using Cloudents.Query.Query.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Ioc inject")]
    public class CashOutQueryHandler : IQueryHandler<AdminEmptyQuery, IEnumerable<CashOutDto>>
    {
        private readonly DapperRepository _session;



        public CashOutQueryHandler(DapperRepository session)
        {
            _session = session;
        }

        public async Task<IEnumerable<CashOutDto>> GetAsync(AdminEmptyQuery query, CancellationToken token)
        {
            using (var conn = _session.OpenConnection())
            {
                var result = await conn.QueryAsync<CashOutDto>(@"select t.User_id as userId,
t.price as CashoutPrice,
u.Email as UserEmail,
t.created as CashOutTime,
t.Approved as Approved,
t.declinedReason DeclinedReason,
t.id as TransactionId,
(case when u.country = 'IL' then 1 else 0 end) as IsIsrael,
(select count(*) from sb.[Transaction] v where TransactionType = 'Refer' and t.User_id = v.User_id) as ReferCount,
(select count(*) from sb.[Transaction] v where TransactionType = 'Document' and v.type = 'earned' and t.User_id = v.User_id) as SoldDocument,
(select count(*) from sb.[Transaction] v where TransactionType = 'Question' and v.type = 'earned' and t.User_id = v.User_id) as CorrectAnswer,
(select count(*) from sb.[Transaction] v left join sb.Document d on v.DocumentId = d.Id  where coalesce( d.State,'deleted') = 'Deleted' and TransactionType = 'Document' and v.type = 'earned' and t.User_id = v.User_id) as SoldDeletedDocument,
(select count(*) from sb.[Transaction] v left join sb.Answer a on v.AnswerId = a.Id where coalesce( a.State,'deleted') = 'Deleted' and TransactionType = 'Question' and v.type = 'earned' and t.User_id = v.User_id) as DeletedCorrectAnswer,
--count cashouts
(select count(*) from sb.[Transaction] v where t.User_id = v.User_id and v.Action = 'CashOut' and v.TransactionType='CashOut') as CashOut,
--count awarded
(select count(*) from sb.[Transaction] v where t.User_id = v.User_id  and v.TransactionType='Award') as AwardCount,
--count buy
(select count(*) from sb.[Transaction] v where t.User_id = v.User_id  and v.TransactionType='BuyPoints') as BuyCount
from sb.[Transaction] t
left outer join sb.[User] u 
on t.User_id = u.Id 
where t.TransactionType='CashOut' and t.Action='CashOut' and t.Approved is null
and t.Created>@date
order by t.id desc", new {date = DateTime.UtcNow.AddMonths(-1)});
                return result;
            }

            //var twoWeeks = TimeSpan.FromDays(14);

            //var sqlQuery = _session.CreateSQLQuery(@"select A.UserId as UserId
            //                                            ,cast(count(distinct Q.UserId) as decimal) / count(distinct Q.id) as userQueryRatio 
            //                                        from sb.[Transaction] T1 
            //                                        inner join sb.Answer A 
            //                                            on A.UserId = T1.[User_id] and T1.[Action] = 'CashOut' 
            //                                        inner join sb.Question Q 
            //                                            on Q.CorrectAnswer_id = A.id 
            //                                        where T1.Created > DATEADD(WEEK, -2, getdate()) 
            //                                        group by A.UserId, T1.Created 
            //                                        order by T1.Created desc"
            //                                        )
            //.SetResultTransformer(Transformers.AliasToBean<FirstQuery>())
            //.Future<FirstQuery>();

            //var futureDto = _session.Query<CashOutTransaction>()
            //    .Fetch(f => f.User)
            //    .Where(w => w.Action == TransactionActionType.CashOut)
            //    .Where(w => w.Created > DateTime.Now - twoWeeks)
            //    .Select(s => new SecondQuery
            //    {
            //        UserId = s.User.Id,
            //        Email = s.User.Email,
            //        Price = s.Price,
            //        Created = s.Created,
            //        FraudScore = s.User.FraudScore,
            //        IsIsrael = s.User.Country == "IL",
            //        Approved = s.Approved,
            //        DeclinedReason = s.DeclinedReason,
            //        TransactionId = s.Id

            //    })

            //   .OrderByDescending(o => o.Created)
            //   .ToFuture();



            //var z = (await futureDto.GetEnumerableAsync(token)).ToArray();
            //var t = sqlQuery.GetEnumerable().ToArray();
            //var tempRes = new List<CashOutDto>();

            //for (var i = 0; i < t.Length; i++)
            //{
            //    var index = z.ElementAt(i);
            //    var tIndex = t.ElementAt(i);

            //    tempRes.Add(new CashOutDto
            //    {

            //        UserId = index.UserId,
            //        UserEmail = index.Email,
            //        CashOutPrice = index.Price,
            //        CashOutTime = index.Created,
            //        FraudScore = index.FraudScore,
            //        UserQueryRatio = tIndex.UserQueryRatio,
            //        IsIsrael = index.IsIsrael,
            //        Approved = index.Approved,
            //        DeclinedReason = index.DeclinedReason,
            //        TransactionId = index.TransactionId
            //    });

            //}
            //return tempRes;
        }
    }
}
