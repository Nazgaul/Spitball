using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Query.Admin
{
    public class AdminCourseSearchQuery : IQueryAdmin<IEnumerable<CourseDto>>
    {
        public AdminCourseSearchQuery(long userId, string term, int page, string country)
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
        private string Term { get; }
        private int Page { get; }
        public string Country { get; }



        internal sealed class AdminCourseSearchQueryHandler : IQueryHandler<AdminCourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public AdminCourseSearchQueryHandler (IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(AdminCourseSearchQuery query, CancellationToken token)
            {
                const int pageSize = 50;
                const string sql =
                            @"     
Select @Term = case when @Term is null then '""""' else '""' + @Term+ '*""' end 
select c.Name,
	case when uc.CourseId is not null then 1 else null end as IsFollowing,
	c.count as Students
from sb.Course c
left join sb.UsersCourses uc
	on c.Name = uc.CourseId and uc.UserId = @Id
where (@Term = '""""' or Contains(Name,  @Term))
and (c.name in (
				select CourseId 
				from sb.UsersCourses uc1 
				join sb.[user] u 
					on u.Id = uc1.UserId
				where (u.Country = @Country or @Country is null)
				and uc1.CourseId = c.Name
				)
	or 
	(select count(1) from sb.UsersCourses uc1 where uc1.CourseId = c.Name) = 0
	)
and State = 'OK'
order by case when uc.CourseId is not null
        then 1 else null end desc,
		c.count desc				
OFFSET @PageSize * @Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new
                    {
                        query.Term,
                        query.Country,
                        Id = query.UserId,
                        PageSize = pageSize,
                        query.Page
                    });
                }
            }
        }
    }
}
