using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminSessionsQuery: IQueryAdmin<IEnumerable<SessionDto>>
    {
        public AdminSessionsQuery(long userId, string country)
        {
            UserId = userId;
            Country = country;
        }
        private long UserId { get; set; }
        public string Country { get; set; }

        internal sealed class AdminSessionsQueryHandler : IQueryHandler<AdminSessionsQuery, IEnumerable<SessionDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminSessionsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<SessionDto>> GetAsync(AdminSessionsQuery query, CancellationToken token)
            {
                const string sql = @"select cast (S.created as date) as Created,
		                                    sum (datediff(minute, S.[Created],S.ended)) as Duration,
		                                    T.Name as Tutor,
		                                    U.Name as Student
                                    from [sb].[StudyRoomSession] S 
                                    join [sb].[StudyRoom] R
	                                    on S.Studyroomid=R.id
                                    join sb.[user] T 
	                                    on T.Id = R.TutorId
                                    join sb.StudyRoomUser sru
	                                    on sru.StudyRoomId = R.Id and sru.UserId != T.Id
                                    join sb.[user] u
	                                    on sru.UserId = U.Id
                                    where S.Ended is not null and u.Country = @Country
                                    group by cast (S.created as date),T.Name,
		                                    U.Name
                                    order by cast (S.created as date) desc;";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<SessionDto>(sql, new { query.UserId, query.Country});
                    return res;
                }
            }
        }
    }
}
