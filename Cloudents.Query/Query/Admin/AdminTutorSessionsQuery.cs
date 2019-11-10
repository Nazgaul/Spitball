using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminTutorSessionsQuery : IQueryAdmin<IEnumerable<SessionBillDto>>
    {
        public AdminTutorSessionsQuery(long tutorId, string country)
        {
            TutorId = tutorId;
            Country = country;
        }
        public string Country { get; }
        public long TutorId { get; }

        internal sealed class AdminTutorSessionsQueryHandler : IQueryHandler<AdminTutorSessionsQuery, IEnumerable<SessionBillDto>>
        {
            private readonly IDapperRepository _dapper;

            public AdminTutorSessionsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<SessionBillDto>> GetAsync(AdminTutorSessionsQuery query, CancellationToken token)
            {
                var sql = @"select u.Name, u.PhoneNumberHash, u.Email, srs.Created, srs.Ended, 
                            DATEDIFF(MINUTE, srs.Created, srs.Ended) as 'Minutes',
                            cast(DATEDIFF(MINUTE, srs.Created, srs.Ended) as float)*t.Price/60 as Cost,
                        case when Receipt is null then 0 else 1 end as IsPayed
                        from sb.StudyRoom sr
                        join sb.StudyRoomSession srs
	                        on sr.Id = srs.StudyRoomId
                        join sb.StudyRoomUser sru
	                        on sr.Id = sru.StudyRoomId and sru.UserId != sr.TutorId
                        join sb.[User] u
	                        on u.Id = sru.UserId
                        join sb.Tutor t
	                        on t.Id = sr.TutorId
                        where sr.TutorId = @TutorId and (u.Country = @Country or @Country is null)";

                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryAsync<SessionBillDto>(sql, new { query.TutorId, query.Country });
                }
            }

        }
    }
}
