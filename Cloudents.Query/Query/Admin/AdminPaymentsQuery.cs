﻿using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPaymentsQuery : IQuery<IEnumerable<PaymentDto>>
    {
        internal sealed class AdminPaymentsQueryHandler : IQueryHandler<AdminPaymentsQuery, IEnumerable<PaymentDto>>
        {
            private readonly DapperRepository _repository;

            public AdminPaymentsQueryHandler(DapperRepository repository)
            {
                _repository = repository;
            }

            public async Task<IEnumerable<PaymentDto>> GetAsync(AdminPaymentsQuery query, CancellationToken token)
            {

                //This query will not work in case there will be more then one student in a room.
                var sql = @"select srs.Id as StudyRoomSessionId, t.Price*DATEDIFF(MINUTE, srs.Created, srs.Ended)/60 as Price, 
		                        t.SellerKey, 
		                        u.PaymentKey, 
		                        t.Id as TutorId, 
		                        tu.Name as TutorName, 
		                        u.Id as UserId,
		                        u.Name as UserName
                        from [sb].[StudyRoomSession] srs
                        join sb.StudyRoom sr
	                        on srs.StudyRoomId = sr.Id
                        join sb.Tutor t
	                        on sr.TutorId = t.Id
                        join sb.StudyRoomUser sru
	                        on srs.StudyRoomId = sru.StudyRoomId and sru.userId != t.Id
                        join sb.[user] u
	                        on u.id = sru.UserId
                        join sb.[User] tu
	                        on t.Id = tu.Id
                        where Receipt is null and srs.Ended is not null
                            and t.Price*DATEDIFF(MINUTE, srs.Created, srs.Ended)/60 > 10
						    and u.PaymentKey is not null";
                using (var conn = _repository.OpenConnection())
                    {
                        return await conn.QueryAsync<PaymentDto>(sql);
                    
                    }
            }
        }
    }
}
