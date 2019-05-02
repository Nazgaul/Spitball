using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class TutorListTabSearchQuery : IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListTabSearchQuery(string term, int page)
        {
            Term = term.Replace('"', ' ');
            Page = page;
        }

        public string Term { get; }
        public int Page { get; private set; }

        internal sealed class TutorListTabSearchQueryHandler : IQueryHandler<TutorListTabSearchQuery, IEnumerable<TutorListDto>>
        {
            private readonly DapperRepository _dapperRepository;

            public TutorListTabSearchQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }
            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListTabSearchQuery query, CancellationToken token)
            {
                using (var conn = _dapperRepository.OpenConnection())
                {
                    const string sql = @"

Select @Term = '""' + @Term+ '*""'  ;
with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)
Select distinct u.id as UserId,
u.Name,
u.Image,
(select STRING_AGG(dt.CourseId, ', ') FROM sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) as courses,
T.Price, 
cte.rate as Rate,
t.Bio,
cte.rateCount as ReviewsCount
from sb.tutor t join sb.[user] u on t.Id = u.Id left join cte on t.Id = cte.Id
left join sb.UsersCourses tc on u.id = tc.UserId and tc.CanTeach = 1
left join sb.Course c on tc.CourseId = c.Name
left join sb.CourseSubject cs on c.SubjectId = cs.Id
where ( contains(u.Name,@term) or  contains(t.Bio,@term) or contains(c.Name,@term)  or contains(cs.Name,@term))
order by u.id
OFFSET 50*@Page ROWS
FETCH NEXT 50 ROWS ONLY;";
                    return await conn.QueryAsync<TutorListDto>(sql, new {query.Page, query.Term});
                }
            }
        }
    }
}