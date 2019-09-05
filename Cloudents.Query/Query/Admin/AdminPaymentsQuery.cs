﻿using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPaymentsQuery : IQuery<IEnumerable<PaymentDto>>
    {
        internal sealed class AdminPaymentsQueryHandler : IQueryHandler<AdminPaymentsQuery, IEnumerable<PaymentDto>>
        {
            private readonly IDapperRepository _repository;

            public AdminPaymentsQueryHandler(IDapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(AdminPaymentsQuery query, CancellationToken token)
            {
                //This query will not work in case there will be more then one student in a room.
                const string sql = @"select srs.Id as StudyRoomSessionId,
                    case when t.Price is null then tr.Price else t.Price end as Price,
                    case when tr.SellerKey is null then 1 else 0 end as cantPay,
		                    tr.Id as TutorId, 
		                    tu.Name as TutorName, 
		                    u.Id as UserId,
		                    u.Name as UserName,
		                    srs.Created,
							srs.DurationInMinutes as Duration
                    from [sb].[StudyRoomSession] srs
                    join sb.StudyRoom sr
	                    on srs.StudyRoomId = sr.Id
                    left join sb.TutorHistory t
	                    on sr.TutorId = t.Id and srs.Created between t.BeginDate and t.EndDate
                    join sb.Tutor tr
	                    on tr.Id = sr.TutorId
                    join sb.StudyRoomUser sru
	                    on srs.StudyRoomId = sru.StudyRoomId and sru.userId != tr.Id
                    join sb.[user] u
	                    on u.id = sru.UserId
                    join sb.[User] tu
	                    on tr.Id = tu.Id
                    where Receipt is null
                        and srs.DurationInMinutes > 10
	                    and u.PaymentKey is not null";
                using (var conn = _repository.OpenConnection())
                {
                    return await conn.QueryAsync<PaymentDto>(sql);

                }
            }
        }
    }
}
