using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminDailyStudyRoomQuery : IQuery<IEnumerable<DailyStudyRoomsDto>>
    {
        internal class AdminDailyStudyRoomQueryHandler : IQueryHandler<AdminDailyStudyRoomQuery, IEnumerable<DailyStudyRoomsDto>>
        {

            private readonly DapperRepository _dapper;


            public AdminDailyStudyRoomQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<DailyStudyRoomsDto>> GetAsync(AdminDailyStudyRoomQuery query, CancellationToken token)
            {
                const string sql = @"Select	cast(S.[Created] as date) as 'Day', 
                                        count(1) as 'Sessions', 
                                        count(distinct T.Id) as Tutors, 
                                        count(distinct u.Id) as Users
                                    from [sb].[StudyRoomSession] S 
                                    join [sb].[StudyRoom] R
	                                    on S.Studyroomid=R.id
                                    Join sb.[User] T 
	                                    on T.Id = R.TutorId
                                    join sb.StudyRoomUser sru
	                                    on sru.StudyRoomId = R.Id and sru.UserId != T.Id
                                    Join Sb.[user] u
	                                    on sru.UserId = U.Id
                                    where U.email not like '%cloudents%'
                                    and U.email not like '%spitball%'
                                    and (datediff(minute, S.[Created],S.ended) != 0 or datediff(minute, S.[Created],S.ended) is null)
                                    group by cast(S.[Created] as date) 
                                    order by cast(S.[Created] as date)  desc";
                using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryAsync<DailyStudyRoomsDto>(sql, token);
                    //return res.AsList();
                }
            }
        }
    }
}
