﻿using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudents.Query.Courses
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
                const int pageSize = 30;
                const string sql =
                            @"declare @country nvarchar(2) = (select country from sb.[user] where id = @Id);




with cte as 
(
select distinct CourseId
		from sb.UsersCourses uc1 
		join sb.[user] u1 
			on u1.Id = uc1.UserId 
		
		where u1.Country = @Country
)

select Name,
	1 as IsFollowing,
	c.count as Students
from sb.Course c
join sb.UsersCourses uc
	on c.Name = uc.CourseId and uc.UserId = @Id
union all
select Name,
	0 as IsFollowing,
	c.count as Students
from sb.Course c

where exists (select CourseId from cte where CourseId = c.Name)
and not exists (select uc.CourseId from sb.UsersCourses uc where c.Name = uc.CourseId and uc.UserId = @Id)
and (c.Country is null or c.Country = @Country)
order by IsFollowing desc,
		c.count desc
OFFSET @PageSize * @Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                using var conn = _dapperRepository.OpenConnection();
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