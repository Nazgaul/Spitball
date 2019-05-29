using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
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
                var sql = @"Select	U1.[name] TutorName, 
		                            U2.[Name] UserName, 
		                            S.created Created, 
		                            datediff(minute, S.[Created],S.ended) Duration
                            from [sb].[StudyRoomSession] S 
                            join [sb].[StudyRoom] R
	                            on S.Studyroomid=R.id
                            Join sb.[User] U1 
	                            on U1.id=Left (Identifier, CHARINDEX('_', Identifier)-1)
                            Join Sb.[user] U2 
	                            on U2.id=Right (Identifier, len(Identifier)-CHARINDEX('_', Identifier))
                            where S.Ended is not null 
                            and U1.email not like '%cloudents%'
                            and U1.email not like '%spitball%'
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
