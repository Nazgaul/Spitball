using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Admin
{
    public class CourseSearchQuery : IQueryAdmin2<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(long userId, string term, int page, Country country)
        {
            UserId = userId;
            if (!string.IsNullOrEmpty(term))
            {
                Term = term.Replace('"', ' ');
            }

            Page = page;
            Country = country;
        }

        private long UserId { get; }
        private string? Term { get; }
        private int Page { get; }
        public Country Country { get; }



        internal sealed class CourseSearchQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public CourseSearchQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {
                const int pageSize = 50;
                const string sql =
                            @"     
Select @Term = case when @Term is null then '""""' else '""' + @Term+ '*""' end 
select c.SearchDisplay as Name,
	case when uc.CourseId is not null then 1 else null end as IsFollowing,
	c.count as Students
from sb.Course2 c
left join sb.UserCourse2 uc
	on c.id = uc.CourseId and uc.UserId = @Id
where (@Term = '""""' or Contains(SearchDisplay,  @Term))
and (c.Country = @Country or @Country is null)
and State = 'OK'
order by case when uc.CourseId is not null
        then 1 else null end desc,
		c.count desc				
OFFSET @PageSize * @Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                using var conn = _dapperRepository.OpenConnection();
                return await conn.QueryAsync<CourseDto>(sql, new
                {
                    query.Term,
                    Country = query.Country.Name,
                    Id = query.UserId,
                    PageSize = pageSize,
                    query.Page
                });
            }
        }
    }
}
