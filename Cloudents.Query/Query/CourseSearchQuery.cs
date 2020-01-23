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
declare @schoolType nvarchar(50) = (select case when UserType = 'University' then 'University'
										when UserType is null then null
										else 'HighSchool' end from sb.[user] where Id = @Id);
								
select Name,
	case when uc.CourseId is not null then 1 else null end as IsFollowing,
	c.count as Students
from sb.Course c
left join sb.UsersCourses uc
	on c.Name = uc.CourseId and uc.UserId = @Id
where  State = 'OK'
	and ( c.SchoolType = @schoolType 
	or (@schoolType = 'University' and c.SchoolType is null)
	or @schoolType is null )
order by case when uc.CourseId is not null
        then 1 else null end desc,
		c.count desc
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