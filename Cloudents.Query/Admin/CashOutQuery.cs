using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class CashOutQuery : IQueryAdmin<IEnumerable<CashOutDto>>
    {
        public CashOutQuery(string? country)
        {
            Country = country;
        }
        public string? Country { get; }
        internal sealed class CashOutEmptyQueryHandler : IQueryHandler<CashOutQuery, IEnumerable<CashOutDto>>
        {
            private readonly IDapperRepository _session;



            public CashOutEmptyQueryHandler(IDapperRepository session)
            {
                _session = session;
            }

            public async Task<IEnumerable<CashOutDto>> GetAsync(CashOutQuery query, CancellationToken token)
            {
                var sql = @"select t.User_id as userId,
t.price as CashoutPrice,
u.Email as UserEmail,
t.created as CashOutTime,
t.Approved as Approved,
t.declinedReason DeclinedReason,
t.id as TransactionId,
u.country as Country,
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
join sb.[User] u 
on t.User_id = u.Id 
where t.TransactionType='CashOut' and t.Action='CashOut' and t.Approved is null and t.Created>@date";
                if (!string.IsNullOrEmpty(query.Country))
                {
                    sql += " and u.Country = @Country";
                }
                sql += " order by t.id desc";
                using var conn = _session.OpenConnection();
                var result = await conn.QueryAsync<CashOutDto>(sql, new { date = DateTime.UtcNow.AddMonths(-1), query.Country });
                return result;
            }
        }
    }
}
