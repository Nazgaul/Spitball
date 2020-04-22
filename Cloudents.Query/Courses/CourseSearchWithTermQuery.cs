using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace Cloudents.Query.Courses
{
    public class CourseSearchWithTermQuery : IQuery<IEnumerable<CourseDto>>
    {
        public CourseSearchWithTermQuery(long userId, string term, int page)
        {
            UserId = userId;
            Term = term.Replace('"', ' ');
            Page = page;
        }

        private long UserId { get; }
        private string Term { get; }
        private int Page { get; }



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

select Name,
	                            case when uc.CourseId is not null then 1 else null end as IsFollowing,
	                            c.count as Students
                            from sb.Course c
                            left join sb.UsersCourses uc
	                            on c.Name = uc.CourseId and uc.UserId = @Id
                             where Contains(Name,  @Term)
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
                    Id = query.UserId,
                    PageSize = pageSize,
                    query.Page
                });
            }
        }
    }

}