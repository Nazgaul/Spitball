using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Query
{
    public class TutorListByCourseQuery: IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId, int page)
        {
            CourseId = courseId;
            UserId = userId;
            Page = page;
        }

        

        private string CourseId { get; }
        private long UserId { get; }
        private int Page { get; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListByCourseQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                const string sql = @"select *  from (select U.Id as UserId, U.Name, U.Image, T.Bio, T.Price, U.Score, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and uc.CourseId = @CourseId

union
select U.Id as UserId, U.Name, U.Image, T.Bio, T.Price, U.Score, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						join sb.Course c on uc.CourseId = c.Name
						and c.SubjectId = (Select subjectId from sb.Course where Name = @CourseId)
) t
where t.UserId <> @UserId
order by Rate desc
OFFSET @page*20 ROWS
FETCH NEXT 20 ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql,new
                    {
                        query.CourseId ,
                        query.UserId,
                        query.Page
                    });

                    return retVal;
                }
            }
        }
    }
}