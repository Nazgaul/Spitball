using Cloudents.Core.DTOs.Email;
using Dapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Email
{
    public class RequestTutorAdminEmailQuery : IQuery<IEnumerable<RequestTutorAdminEmailDto>>
    {
        public RequestTutorAdminEmailQuery(Guid leadId)
        {
            LeadId = leadId;
        }

        private Guid LeadId { get; }

        internal sealed class RequestTutorAdminEmailQueryHandler : IQueryHandler<RequestTutorAdminEmailQuery, IEnumerable<RequestTutorAdminEmailDto>>
        {
            private readonly IDapperRepository _dapper;

            public RequestTutorAdminEmailQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<RequestTutorAdminEmailDto>> GetAsync(RequestTutorAdminEmailQuery query, CancellationToken token)
            {
                const string sql = @"select l.CourseId as CourseName,
	                                    ut.Name as TutorName,
	                                    u.PhoneNumberHash as UserPhone,
	                                    u.Id as UserId
                                    from sb.[Lead] l
                                    join sb.ChatRoomAdmin cra
	                                    on l.Id = cra.LeadId
                                    join sb.ChatRoom cr
	                                    on cra.Id = cr.Id
                                    join sb.ChatUser cu
	                                    on cu.ChatRoomId = cra.Id and cu.UserId != l.UserId
                                    join sb.Tutor t
	                                    on t.Id = cu.UserId
                                    join sb.[User] as ut
	                                    on t.Id = ut.Id
                                    join sb.[user] u
	                                    on l.UserId = u.Id and u.PhoneNumberConfirmed = 1 and u.Country = 'il'
                                    where l.Id =  @LeadId";
                using (var conn = _dapper.OpenConnection())
                {
                    var res = await conn.QueryAsync<RequestTutorAdminEmailDto>(sql, new
                    {
                        leadId = query.LeadId
                    });
                    return res;
                }
            }
        }
    }
}
