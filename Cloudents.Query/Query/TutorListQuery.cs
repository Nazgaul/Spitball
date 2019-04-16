using Cloudents.Core.DTOs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Cloudents.Query.Query
{
    public class TutorListQuery : IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListQuery(long userId, int page)
        {
            UserId = userId;
            Page = page;
        }


        private long UserId { get; }
        private int Page { get; }

        internal sealed class TutorListQueryHandler : IQueryHandler<TutorListQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListQuery query, CancellationToken token)
            {
                const string sql = @"select U.Id as UserId, U.Name, U.Image, T.Bio, T.Price, U.Score, 
	                        (select avg(Rate) from sb.TutorReview where TutorId = T.Id) as Rate
                        from sb.[user] U
                        join sb.Tutor T
	                        on U.Id = T.Id
						join sb.UsersCourses uc on u.Id = uc.UserId and uc.CanTeach = 1
						and  uc.CourseId in (select CourseId from sb.UsersCourses where UserId = @UserId)
and u.id <> @UserId
                        order by Rate desc
OFFSET @page*20 ROWS
FETCH NEXT 20 ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    var retVal = await conn.QueryAsync<TutorListDto>(sql, new
                    {
                        query.UserId,query.Page

                    });

                    return retVal;
                }
            }
        }
    }



}
