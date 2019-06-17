﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.DTOs;
using Dapper;

namespace Cloudents.Query.Tutor
{
    public class TutorListTabQuery : IQuery<IEnumerable<TutorListDto>>
    {
        public TutorListTabQuery(string country, int page)
        {
            Page = page;
            Country = country;
        }

        private int Page { get; }
        private string Country { get; }


        internal sealed class TutorListTabQueryHandler :IQueryHandler<TutorListTabQuery, IEnumerable<TutorListDto>>
        {
            private const int PageSize = 25;
            private readonly DapperRepository _dapperRepository;

            public TutorListTabQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }

            public async Task<IEnumerable<TutorListDto>> GetAsync(TutorListTabQuery query, CancellationToken token)
            {
                using (var conn = _dapperRepository.OpenConnection())
                {
                    const string sql = @"with cte as(
select t.TutorId as Id,  avg(Rate) as rate,count(*) as rateCount from sb.TutorReview t
group by t.TutorId
)
Select u.id as UserId,
u.Name,
u.Image,
(select STRING_AGG(dt.CourseId, ', ') FROM(select top 10 courseId
from sb.UsersCourses dt where u.Id = dt.UserId and dt.CanTeach = 1) dt) as courses,
T.Price, 
cte.rate as Rate,
t.Bio,
cte.rateCount as ReviewsCount
from sb.tutor t join sb.[user] u on t.Id = u.Id left join cte on t.Id = cte.Id
where t.State = 'Ok'
and (u.Country = @Country or @Country is null)
order by
cte.rate desc,
 u.id

OFFSET @PageSize*@Page ROWS
FETCH NEXT @PageSize ROWS ONLY;";
                    return await conn.QueryAsync<TutorListDto>(sql, new {query.Page, query.Country, PageSize });

                }
            }
        }

        
    }
}