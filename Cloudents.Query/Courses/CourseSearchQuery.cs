using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;


namespace Cloudents.Query.Courses
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(long userId, int page, Country country)
        {
            UserId = userId;
            Page = page;
            Country = country;
        }

        private long UserId { get; }
        private int Page { get; }

        private Country Country { get; }



        internal sealed class CourseSearchQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public CourseSearchQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {


                const int pageSize = 30;
                const string sql =
                            @"Select c.SearchDisplay as name,count as Students,
CASE WHEN uc.UserId IS NULL THEN 0 ELSE 1 END  as  IsFollowing from sb.Course2 c
left join sb.UserCourse2 uc on c.Id = uc.CourseId and uc.UserId = @Id
where c.country = @Country and State = 'OK'
order by uc.UserId desc ,c.[Count] desc
OFFSET @PageSize * @Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<CourseDto>(sql, new
                {
                    Id = query.UserId,
                    PageSize = pageSize,
                    Country = query.Country.Id,
                    query.Page
                });
            }
        }
    }

}