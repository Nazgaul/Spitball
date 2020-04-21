using Cloudents.Core.DTOs.Admin;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Admin
{
    public class PendingTutorsQuery : IQueryAdmin2<IEnumerable<PendingTutorsDto>>
    {
        public PendingTutorsQuery(Country? country)
        {
            Country = country;
        }

        public Country? Country { get; }

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
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt 
where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
u.ImageName as Image
from sb.[User] u
join sb.Tutor t
	on u.Id = t.Id 
where t.State = 'Pending' AND (@Country IS NULL OR u.SbCountry = @Country) order by t.id desc OPTION(RECOMPILE)";


                using var connection = _dapper.OpenConnection();
                var res = await connection.QueryAsync<PendingTutorsDto>(sql,
                    new { Country = query.Country?.Id });
                return res;
            }
        }
    }
}
