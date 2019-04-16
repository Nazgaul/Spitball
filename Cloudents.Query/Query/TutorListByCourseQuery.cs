using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Query
{
    public class TutorListByCourseQuery: IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListByCourseQuery(string courseId, long userId)
        {
            CourseId = courseId;
            UserId = userId;
        }

        

        private string CourseId { get; }
        public long UserId { get; }

        internal sealed class TutorListByCourseQueryHandler : IQueryHandler<TutorListByCourseQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListByCourseQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListByCourseQuery query, CancellationToken token)
            {
                const string sql = @"select top 10 U.Id as UserId, U.Name, U.Image, T.Bio, T.Price, U.Score, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and uc.CourseId = @CourseId
and u.id <> @UserId
                        order by Rate desc";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql,new
                    {
                        query.CourseId ,
                        query.UserId
                    });

                    return retVal;
                }
            }
        }
    }
}