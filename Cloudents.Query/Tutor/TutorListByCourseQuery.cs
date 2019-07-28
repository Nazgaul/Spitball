using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class TutorListByCourseQuery: IQuery<IEnumerable<TutorCardDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId, int count)
        {
            CourseId = courseId;
            UserId = userId;
            Count = count;
        }

        
        /// <summary>
        /// The course name we want to get tutors
        /// </summary>
        private string CourseId { get; }
        /// <summary>
        /// Eliminate the current user from the result
        /// </summary>
        private long UserId { get; }

        private int Count { get; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorCardDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListByCourseQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorCardDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                const string sql = @"select *  from (select 2 as position, U.Id as UserId, U.Name, U.Image,
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
T.Bio,
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and uc.CourseId = @CourseId
                        and T.State = 'Ok'

union all
select 1 as position, U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
T.Bio,
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate,
                            (select count(1) from sb.TutorReview where TutorId = T.Id) as ReviewsCount
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						join sb.Course c on uc.CourseId = c.Name
						and c.SubjectId = (Select subjectId from sb.Course where Name = @CourseId)
                        and T.State = 'Ok'
) t
where t.UserId <> @UserId
order by position desc, Rate desc
OFFSET 0 ROWS
FETCH NEXT @Count ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorCardDto>(sql,new
                    {
                        query.CourseId ,
                        query.UserId,
                        query.Count
                    });

                    return retVal.Distinct(TutorCardDto.UserIdComparer);
                }
            }
        }
    }
}