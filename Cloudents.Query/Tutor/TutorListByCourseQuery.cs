using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class TutorListByCourseQuery: IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId)
        {
            CourseId = courseId;
            UserId = userId;
        }

        

        private string CourseId { get; }
        private long UserId { get; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListByCourseQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                const string sql = @"select *  from (select 2 as position, U.Id as UserId, U.Name, U.Image,
(select STRING_AGG(dt.CourseId, ', ') FROM sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and uc.CourseId = @CourseId
                        and T.State = @state

union
select 1 as position, U.Id as UserId, U.Name, U.Image, 
(select STRING_AGG(dt.CourseId, ', ') FROM sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) as courses,
T.Price, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						join sb.Course c on uc.CourseId = c.Name
						and c.SubjectId = (Select subjectId from sb.Course where Name = @CourseId)
                        and T.State = @state
) t
where t.UserId <> @UserId
order by position desc, Rate desc
OFFSET 0 ROWS
FETCH NEXT 20 ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql,new
                    {
                        query.CourseId ,
                        query.UserId,
                        state = ItemState.Ok.ToString("G")
                    });

                    return retVal;
                }
            }
        }
    }
}