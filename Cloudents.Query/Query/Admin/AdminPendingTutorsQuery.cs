using Cloudents.Core.DTOs.Admin;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using Dapper;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
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
                const string sql = @"select u.Id, u.FirstName, u.LastName, u.Email, t.Bio, t.Price, STRING_AGG(uc.CourseId, ', ') as Courses
                                    from sb.[User] u
                                    join sb.Tutor t
	                                    on u.Id = t.Id
                                    left join sb.UsersCourses uc
	                                    on u.Id = uc.UserId and CanTeach = 1
                                    where t.State = 'Pending'
                                    group by u.Id, u.FirstName, u.LastName, u.Email, t.Bio, t.Price";
                using (var connection = _dapper.OpenConnection())
                {
                    var res = await connection.QueryAsync<PendingTutorsDto>(sql);
                    return res;
                }
            }
        }
    }
}
