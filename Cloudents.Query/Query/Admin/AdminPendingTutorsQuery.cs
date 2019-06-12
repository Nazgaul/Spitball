﻿using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminPendingTutorsQuery: IQuery<IEnumerable<PendingTutorsDto>>
    {
        internal sealed class AdminPendingTutorsQueryHandler : IQueryHandler<AdminPendingTutorsQuery, IEnumerable<PendingTutorsDto>>
        {
            private readonly DapperRepository _dapper;
            public AdminPendingTutorsQueryHandler(DapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<PendingTutorsDto>> GetAsync(AdminPendingTutorsQuery query, CancellationToken token)
            {

                const string sql = @"select u.Id, u.FirstName, u.LastName,
u.Email, t.Bio, t.Price, t.Created, (select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
u.image
                                    from sb.[User] u
                                    join sb.Tutor t
	                                    on u.Id = t.Id
                                    where t.State = 'Pending'";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<PendingTutorsDto>(sql);
                    return res;
                }
               
            }
        }
    }
}
