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


            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                const string sql = @"select distinct U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and  uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId or @UserId = 0)
                        and T.State = 'Ok'
                        and (U.Country = @Country or @Country is null)
where t.Id <> @UserId or @UserId = 0
order by Rate desc
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;


select distinct U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						join sb.Course c on uc.CourseId = c.Name
						and c.SubjectId in (Select subjectId  from sb.UsersCourses where UserId = @UserId or @UserId = 0)
						and T.State = 'Ok'
						and (U.Country = @Country or @Country is null)
where t.Id <> @UserId or @UserId = 0
order by Rate desc
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;


select distinct U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						and T.State = 'Ok'
						and (U.Country = @Country or @Country is null)
where t.Id <> @UserId or @UserId = 0
order by Rate desc
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;";


                using (var conn = _dapperRepository.OpenConnection())
                {
                    using (var multi = await conn.QueryMultipleAsync(sql, new { query.UserId, query.Country }))
                    {
                        var t1 = await multi.ReadAsync<TutorListDto>();
                        var t2 = await multi.ReadAsync<TutorListDto>();
                        var t3 = await multi.ReadAsync<TutorListDto>();

                        return t1.Union(t2).Union(t3).Distinct(TutorListDto.UserIdComparer).Take(20);
                    }

                }
            }
        }
    }



}
