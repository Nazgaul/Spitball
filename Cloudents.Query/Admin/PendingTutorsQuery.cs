using Cloudents.Core.DTOs.Admin;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using NHibernate;
using NHibernate.Linq;

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
            private readonly IStatelessSession _session;
            public PendingTutorsQueryHandler(IStatelessSession session)
            {
                _session = session;
            }

            public async Task<IEnumerable<PendingTutorsDto>> GetAsync(PendingTutorsQuery query, CancellationToken token)
            {
                var tutorQuery = _session.Query<Core.Entities.Tutor>()
                   // .Fetch(f=>f.User)
                   // .ThenFetchMany(f=>f.UserCourses)
                    .Where(w => w.State == ItemState.Pending);

                if (query.Country != null)
                {
                    tutorQuery = tutorQuery.Where(w => w.User.SbCountry == query.Country);
                }
                //THIS Query is not optimum
                return await tutorQuery.OrderByDescending(t=>t.Id).Select(s => new PendingTutorsDto
                {
                    FirstName = s.User.FirstName,
                    Email = s.User.Email,
                    LastName = s.User.LastName,
                    Bio = s.Paragraph2,
                    Image = s.User.ImageName,
                    Created = s.Created,
                    Id = s.Id,
                    Courses2 = s.User.UserCourses.Select(s2 => s2.Course.Id)
                }).ToListAsync(token);

//                const string sql = @"select u.Id, u.FirstName, u.LastName,
//u.Email, t.Bio, t.Created, 
//(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
//from sb.UsersCourses dt 
//where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
//u.ImageName as Image
//from sb.[User] u
//join sb.Tutor t
//	on u.Id = t.Id 
//where t.State = 'Pending' AND (@Country IS NULL OR u.SbCountry = @Country) order by t.id desc OPTION(RECOMPILE)";


//                using var connection = _dapper.OpenConnection();
//                var res = await connection.QueryAsync<PendingTutorsDto>(sql,
//                    new { Country = query.Country?.Id });
//                return res;
            }
        }
    }
}
