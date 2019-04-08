using Cloudents.Core.DTOs;
using Dapper;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    public class DocumentAggregateQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public DocumentAggregateQuery(long userId, int page)
        {
            Page = page;
            UserId = userId;
        }

        public int Page { get; private set; }

        public long UserId { get; private set; }


        internal sealed class DocumentAggregateQueryHandler : IQueryHandler<DocumentAggregateQuery, IEnumerable<DocumentFeedDto>>
        {

            private readonly DapperRepository _dapperRepository;

            public DocumentAggregateQueryHandler(DapperRepository dapperRepository)
            {
                _dapperRepository = dapperRepository;
            }


            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(DocumentAggregateQuery query, CancellationToken token)
            {
                const string sql = @"with cte as (
select u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
  from sb.[user] u left join sb.University u2 on u.UniversityId2 = u2.Id
  where u.id = @userid 
)
select case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
case when un.Country = cte.Country then 2 else 0 end +
cast(1 as float)/DATEDIFF(day, d.updateTime, GETUTCDATE()),d.Id
,un.Name as University
,d.CourseName as Course
,d.MetaContent as Snippet
,d.Professor
,d.Type
,u.Id
,U.Name
,u.Score
,u.Image
,d.[Views]
,d.Downloads
,d.CreationTime as [DateTime]
,d.Price as Price
,(select sum(v.VoteType) from sb.Vote v where d.Id = v.DocumentId) as Vote
from sb.Document d 
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId,
cte
where d.CourseName in (select courseId from sb.usersCourses where userid = cte.userid)
order by case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
case when un.Country = cte.Country then 2 else 0 end +
cast(1 as float)/DATEDIFF(day, d.updateTime, GETUTCDATE()) desc
OFFSET @page*50 ROWS
FETCH NEXT 50 ROWS ONLY";

                using (var conn = _dapperRepository.OpenConnection())
                {
                    var result = await conn.QueryAsync<DocumentFeedDto>(sql, new {@page = query.Page, userid = query.UserId});
                    return result;
                }
            }
        }
    }
}