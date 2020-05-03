using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class StudyRoomQuery : IQueryAdmin<IEnumerable<StudyRoomDto>>
    {
        public StudyRoomQuery(string country)
        {
            Country = country;
        }
        public string? Country { get; }
        internal sealed class StudyRoomQueryHandler : IQueryHandler<StudyRoomQuery, IEnumerable<StudyRoomDto>>
        {

            private readonly IDapperRepository _dapper;


            public StudyRoomQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<StudyRoomDto>> GetAsync(StudyRoomQuery query, CancellationToken token)
            {
                var sql = @"Select	S.Id as SessionId,
                                    T.[name] TutorName, 
		                            U.[Name] UserName, 
		                            S.created Created, 
		                            isnull(cast(DATEDIFF(minute, s.Created, s.Ended) as nvarchar(10)), 'OnGoing') Duration,
		                            R.TutorId,
                                    U.Id as UserId
                            from [sb].[StudyRoomSession] S 
                            join [sb].[StudyRoom] R
	                            on S.StudyRoomId=R.id
                            Join sb.[User] T 
	                            on T.Id = R.TutorId
                            join sb.StudyRoomUser sru
	                            on sru.StudyRoomId = R.Id and sru.UserId != T.Id
                            Join Sb.[user] u
	                            on sru.UserId = U.Id
                            where U.email not like '%cloudents%'
                            and U.email not like '%spitball%'
                            and (DATEDIFF(minute, s.Created, s.Ended) != 0 or DATEDIFF(minute, s.Created, s.Ended) is null)";
                if (!string.IsNullOrEmpty(query.Country))
                {
                    sql += " and T.Country = @Country";
                }
                sql += " order by S.created desc, S.Id";

                using var connection = _dapper.OpenConnection();
                return await connection.QueryAsync<StudyRoomDto>(sql, new { query.Country });
                //return res.AsList();
            }
        }
    }
}
