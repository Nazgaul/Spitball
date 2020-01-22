using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class SessionsQuery : IQueryAdmin<IEnumerable<SessionDto>>
    {
        public SessionsQuery(long userId, string country)
        {
            UserId = userId;
            Country = country;
        }
        private long UserId { get; }
        public string Country { get; }

        internal sealed class SessionsQueryHandler : IQueryHandler<SessionsQuery, IEnumerable<SessionDto>>
        {
            private readonly IDapperRepository _dapper;


            public SessionsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<SessionDto>> GetAsync(SessionsQuery query, CancellationToken token)
            {
                string sql = @"select cast (S.created as date) as Created,
		                                    sum(s.DurationInMinutes) as Duration,
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
                                    where S.Ended is not null and u.Id = @UserId";

                if (!string.IsNullOrEmpty(query.Country))
                {
                    sql += " and u.Country = @Country";
                }
                sql += @" group by cast (S.created as date),T.Name,U.Name
                                    order by cast(S.created as date) desc;";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<SessionDto>(sql, new { query.UserId, query.Country });
                    return res;
                }
            }
        }
    }
}
