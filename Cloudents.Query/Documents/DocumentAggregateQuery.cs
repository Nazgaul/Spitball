using Cloudents.Core.DTOs;
using Dapper;
using NHibernate;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Query.Stuff;

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

            private readonly IStatelessSession _dapperRepository;

            public DocumentAggregateQueryHandler(QuerySession dapperRepository)
            {
                _dapperRepository = dapperRepository.StatelessSession;
            }


            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(DocumentAggregateQuery query, CancellationToken token)
            {
                const string sql = @"with cte as (
select u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
  from sb.[user] u left join sb.University u2 on u.UniversityId2 = u2.Id
  where u.id = :userid 
)
select 
d.Id
,un.Name as University
,d.CourseName as Course
,d.MetaContent as Snippet
,d.Professor
,d.Type
,d.Name as Title
,u.Id as User_Id
,U.Name as User_Name
,u.Score as User_Score
,u.Image as User_Image
,d.[Views]
,d.Downloads
,d.CreationTime as [DateTime]
,d.VoteCount as Vote_Votes
,d.Price as Price

from sb.Document d 
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId,
cte
where d.CourseName in (select courseId from sb.usersCourses where userid = cte.userid)
order by case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
case when un.Country = cte.Country then 2 else 0 end +
cast(1 as float)/DATEDIFF(day, d.updateTime, GETUTCDATE()) desc
OFFSET :page*50 ROWS
FETCH NEXT 50 ROWS ONLY";


                const string filter = @"select distinct [Type]
                from sb.Document d
                    where d.CourseName in (select courseid from sb.userscourses where userid = :userid )";


                var sqlQuery = _dapperRepository.CreateSQLQuery(sql);
                sqlQuery.SetInt32("page", query.Page);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetResultTransformer(new DeepTransformer<DocumentFeedDto>('_'));
                var future = sqlQuery.Future<DocumentFeedDto>();


                var filterQuery = _dapperRepository.CreateSQLQuery(filter);
                filterQuery.SetInt64("userid", query.UserId);
                var filtersFuture = filterQuery.Future<string>();


                var filters = await filtersFuture.GetEnumerableAsync(token);
                return await future.GetEnumerableAsync(token);
                //using (var conn = _dapperRepository.OpenConnection())
                //{
                //    //using (var grid = await conn.QueryMultipleAsync(sql))
                //    //{

                //    //}
                //    var result = await conn.QueryAsync<DocumentFeedDto>(sql, new { @page = query.Page, userid = query.UserId });
                //    return result;
                //}
            }
        }
    }
}