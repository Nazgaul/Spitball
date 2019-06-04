using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminStudyRoomQuery: IQuery<IEnumerable<StudyRoomDto>>
    {
        internal class AdminPendingUniversitiesQueryHandler : IQueryHandler<AdminStudyRoomQuery, IEnumerable<StudyRoomDto>>
        {

            private readonly DapperRepository _dapper;


            public AdminPendingUniversitiesQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<StudyRoomDto>> GetAsync(AdminStudyRoomQuery query, CancellationToken token)
            {
                var sql = @"Select	T.[name] TutorName, 
		                            U.[Name] UserName, 
		                            S.created Created, 
		                            datediff(minute, S.[Created],S.ended) Duration,
		                            R.TutorId
                            from [sb].[StudyRoomSession] S 
                            join [sb].[StudyRoom] R
	                            on S.Studyroomid=R.id
                            Join sb.[User] T 
	                            on T.Id = R.TutorId
                            join sb.StudyRoomUser sru
	                            on sru.StudyRoomId = R.Id and sru.UserId != T.Id
                            Join Sb.[user] u
	                            on sru.UserId = U.Id
                            where S.Ended is not null 
                            and U.email not like '%cloudents%'
                            and U.email not like '%spitball%'
                            order by S.created desc, S.Id";

                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryAsync<StudyRoomDto>(sql);
                    //return res.AsList();
                }
            }
        }
    }
}
