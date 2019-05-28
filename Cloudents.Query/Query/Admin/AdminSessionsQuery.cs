using Cloudents.Core.DTOs.Admin;
using Dapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminSessionsQuery: IQuery<IEnumerable<SessionDto>>
    {
        public AdminSessionsQuery(long userId)
        {
            UserId = userId;
        }
        private long UserId { get; set; }

        internal sealed class AdminSessionsQueryHandler : IQueryHandler<AdminSessionsQuery, IEnumerable<SessionDto>>
        {
            private readonly DapperRepository _dapper;


            public AdminSessionsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<SessionDto>> GetAsync(AdminSessionsQuery query, CancellationToken token)
            {
                const string sql = @"Select cast (S.created as date) as Created, U1.[name] As 'Tutor', U2.[Name] As 'Student', 
		sum (datediff(minute, S.[Created],S.ended)) as Duration
	 from [sb].[StudyRoomSession] S join [sb].[StudyRoom] R
		on S.Studyroomid=R.id
		Join sb.[User] U1 on U1.id=Left (Identifier, CHARINDEX('_', Identifier)-1)
		Join Sb.[user] U2 on U2.id=Right (Identifier, len(Identifier)-CHARINDEX('_', Identifier))
		where S.Ended is not null 
		and (u1.Id = @UserId or u2.Id = @UserId)
	group by U1.[name], U2.[Name] , cast (S.created as date) 
	order by cast (S.created as date) desc";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<SessionDto>(sql, new { query.UserId});
                    return res;
                }
            }
        }
    }
}
