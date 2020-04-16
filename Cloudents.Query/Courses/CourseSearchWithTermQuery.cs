using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;


namespace Cloudents.Query.Courses
{
    public class CourseSearchWithTermQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchWithTermQuery(long userId, string term, int page, Country country)
        {
            UserId = userId;
            Term = term.Replace('"', ' ');
            Page = page;
            Country = country;
        }

        private long UserId { get; }
        private string Term { get; }
        private int Page { get; }
        private Country Country { get; }


        internal sealed class CourseSearchWithTermQueryHandler : IQueryHandler<CourseSearchWithTermQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public CourseSearchWithTermQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchWithTermQuery query, CancellationToken token)
            {
                const int pageSize = 30;
                const string sql =
                            @"     
Select @Term =  '""*' + @Term+ '*""'; 

Select c.SearchDisplay as name,count as Students,
CASE WHEN uc.UserId IS NULL THEN 0 ELSE 1 END  as  IsFollowing from sb.Course2 c
left join sb.UserCourse2 uc on c.Id = uc.CourseId and uc.UserId = @Id
where c.country = @Country and State = 'OK' and Contains(SearchDisplay,  @Term)
order by uc.UserId desc ,c.[Count] desc
OFFSET @PageSize * @Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<CourseDto>(sql, new
                {
                    query.Term,
                    Id = query.UserId,
                    PageSize = pageSize,
                    query.Page,
                    Country = query.Country.Id
                });
            }
        }
    }

}