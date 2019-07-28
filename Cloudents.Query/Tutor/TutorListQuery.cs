using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Tutor
{
    public class TutorListQuery : IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListQuery(long userId, string country)
        {
            UserId = userId;
            Country = country;
        }


        private long UserId { get; }
        private string Country { get; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            //TODO: review query 
            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                const string sql = @"select distinct U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price,
T.Bio,
case when uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId or @UserId = 0) then 2
when c.SubjectId in (Select subjectId  from sb.UsersCourses where UserId = @UserId or @UserId = 0) then 1
else 0 end as t,
x.*
	                      
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						left join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						left join sb.Course c on uc.CourseId = c.Name
						
cross apply (select avg(Rate) as Rate, count(1) as ReviewsCount from sb.TutorReview where TutorId = T.Id) as x
where (t.Id <> @UserId or @UserId = 0) 
                        and T.State = 'Ok'
                        and (U.Country = @Country or @Country is null) 
order by t desc, Rate desc
OFFSET 0 ROWS;";


                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql, new { query.UserId, query.Country });
                    var t = retVal.Distinct(TutorListDto.UserIdComparer);
                    return t.Take(20);
                    
                }
            }
        }
    }



}
