﻿using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminStudyRoomQuery: IQueryAdmin<IEnumerable<StudyRoomDto>>
    {
        public AdminStudyRoomQuery(string country)
        {
            Country = country;
        }
        public string Country { get; }
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
		                            isnull(cast(datediff(minute, S.[Created],S.ended) as nvarchar(15)), 'OnGoing') Duration,
		                            R.TutorId,
                                    U.Id as UserId
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
							and (datediff(minute, S.[Created],S.ended) != 0 or datediff(minute, S.[Created],S.ended) is null)";
                if (!string.IsNullOrEmpty(query.Country))
                {
                    sql += " and U.Country = @Country";
                }
                sql += " order by S.created desc, S.Id";

                    using (var connection = _dapper.OpenConnection())
                {
                    return await connection.QueryAsync<StudyRoomDto>(sql, new { query.Country });
                    //return res.AsList();
                }
            }
        }
    }
}
