using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Admin
{
    public class PendingTutorsQuery : IQueryAdmin<IEnumerable<PendingTutorsDto>>
    {
        public PendingTutorsQuery(string country)
        {
            Country = country;
        }

        public string Country { get; }

        internal sealed class PendingTutorsQueryHandler : IQueryHandler<PendingTutorsQuery, IEnumerable<PendingTutorsDto>>
        {
            private readonly IDapperRepository _dapper;
            public PendingTutorsQueryHandler(IDapperRepository dapper)
            {
                _dapper = dapper;
            }

            public async Task<IEnumerable<PendingTutorsDto>> GetAsync(PendingTutorsQuery query, CancellationToken token)
            {
                const string sql = @"select u.Id, u.FirstName, u.LastName,
u.Email, t.Bio, t.Price, t.Created, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 c.SearchDisplay as CourseId
from sb.UserCourse2 dt join sb.Course2 c on dt.CourseId = c.Id
where u.Id = dt.UserId ) dt) as courses,
u.ImageName as Image
from sb.[User] u
join sb.Tutor t
	on u.Id = t.Id 
where t.State = 'Pending' AND (@Country IS NULL OR u.Country = @Country) order by t.id desc OPTION(RECOMPILE)";


                using var connection = _dapper.OpenConnection();
                var res = await connection.QueryAsync<PendingTutorsDto>(sql, new { query.Country });
                return res;
            }
        }
    }
}
