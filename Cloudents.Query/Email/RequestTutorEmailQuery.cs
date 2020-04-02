using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs.Email;
using Dapper;

namespace Cloudents.Query.Email
{
    public class RequestTutorEmailQuery : IQuery<IEnumerable<RequestTutorEmailDto>>
    {
        public RequestTutorEmailQuery(Guid leadId)
        {
            LeadId = leadId;
        }

        private Guid LeadId { get;  }


        internal sealed class RequestTutorEmailQueryHandler : IQueryHandler<RequestTutorEmailQuery, IEnumerable<RequestTutorEmailDto>>
        {

            private readonly IDapperRepository _dapper;

            public RequestTutorEmailQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }
           

            public async Task<IEnumerable<RequestTutorEmailDto>> GetAsync(RequestTutorEmailQuery query, CancellationToken token)
            {
                const string sql = @"select 
                                        u.[Language] as TutorLanguage,
	                                    l.Text as Request,
	                                    us.PhoneNumberHash as StudentPhoneNumber,
	                                    us.Name as StudentName,
                                        us.Id as StudentId,
	                                    l.CourseId as CourseName,
	                                    u.FirstName as TutorFirstName,
	                                    t.Id as TutorId,
	                                    u.Email as TutorEmail,
	                                    u.Country as TutorCountry
                                    from sb.ChatRoomAdmin cra
                                    join sb.[Lead] l
	                                    on cra.LeadId = l.Id
                                    join sb.ChatUser cu
	                                    on cu.ChatRoomId = cra.Id and cu.UserId != l.UserId
                                    join sb.[user] u
	                                    on cu.UserId = u.Id and u.PhoneNumberConfirmed = 1
                                    join sb.Tutor t
	                                    on t.Id = u.Id
                                    join sb.[user] us
	                                    on us.Id = l.UserId
                                    where cra.LeadId = @leadId";
                using var conn = _dapper.OpenConnection();
                var res = await conn.QueryAsync<RequestTutorEmailDto>(sql, new
                {
                    leadId = query.LeadId
                });
                return res;
               
            }
        }
    }
}