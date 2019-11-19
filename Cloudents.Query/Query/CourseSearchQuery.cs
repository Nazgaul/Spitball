using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudents.Query.Query
{
    public class CourseSearchQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchQuery(long userId, int page)
        {
            UserId = userId;
            Page = page;
        }

        private long UserId { get; }
        private int Page { get; }



        internal sealed class CoursesByTermQueryHandler : IQueryHandler<CourseSearchQuery, IEnumerable<CourseDto>>
        {
            private readonly IDapperRepository _dapperRepository;

            public CoursesByTermQueryHandler(IDapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<CourseDto>> GetAsync(CourseSearchQuery query, CancellationToken token)
            {
                const int pageSize = 50;
                const string sql =
                            @"
declare @Country nvarchar(3) = (select country from sb.[user] where Id = @Id);

with cte as 
(
select CourseId, count(1) as Students
		from sb.UsersCourses uc1 
		join sb.[user] u1 
			on u1.Id = uc1.UserId 
		join sb.University un
			on un.Id = u1.UniversityId2
		where un.Country = @Country
		group by CourseId
)

select Name,
	case when uc.CourseId is not null then 1 else null end as IsFollowing,
	c.count as Students
from sb.Course c
join cte on c.Name = cte.CourseId
left join sb.UsersCourses uc
	on c.Name = uc.CourseId and uc.UserId = @Id
where  State = 'OK'
order by case when uc.CourseId is not null
        then 1 else null end desc,
		cte.Students desc
  OFFSET @PageSize * @Page ROWS
  FETCH NEXT @PageSize ROWS ONLY;";
                using (var conn = _dapperRepository.OpenConnection())
                {
                    return await conn.QueryAsync<CourseDto>(sql, new
                    {
                        Id = query.UserId,
                        PageSize = pageSize,
                        query.Page
                    });
                }
            }
        }
    }

}