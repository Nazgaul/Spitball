using Cloudents.Core.DTOs;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    public class DocumentAggregateQuery : IQuery<DocumentFeedWithFacetDto>
    {
        public DocumentAggregateQuery(long userId, int page, string[] filter, string country)
        {
            Page = page;
            UserId = userId;
            Filter = filter;
            Country = country;
        }

        private int Page { get; }

        private long UserId { get; }
        private string[] Filter { get; }

        public string Country { get; private set; }


        internal sealed class DocumentAggregateQueryHandler : IQueryHandler<DocumentAggregateQuery, DocumentFeedWithFacetDto>
        {

            private readonly IStatelessSession _dapperRepository;

            public DocumentAggregateQueryHandler(QuerySession dapperRepository)
            {
                _dapperRepository = dapperRepository.StatelessSession;
            }


            public async Task<DocumentFeedWithFacetDto> GetAsync(DocumentAggregateQuery query, CancellationToken token)
            {
                const string sql = @"with cte as (
select top 1 * from (select 1 as o, u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
  from sb.[user] u 
  left join sb.University u2 on u.UniversityId2 = u2.Id
  where u.id = :userid
  union
  select 2,null,:country,0) t
  order by o
)
select ds.Id
	,ds.University
	,ds.Course
	,ds.Snippet
	,ds.Professor
	,ds.Type
	,ds.Title
	,ds.User_Id
	,ds.User_Name
	,ds.User_Score
	,ds.User_Image
	,ds.[Views]
	,ds.Downloads
	,ds.[DateTime]
	,ds.Vote_Votes
	,(select v.VoteType from sb.Vote v where v.DocumentId = ds.Id and v.UserId = cte.userid) as Vote_Vote
	,ds.Price
from sb.iv_DocumentSearch ds
,cte
where 
     (:typeFilterCount = 0 or ds.Type in (:typefilter))
    and ds.Datetime > GETUTCDATE() - 182 
order by 
case when ds.Course in (select courseId from sb.usersCourses where userid = cte.userid) then 4 else 0 end +
case when ds.UniversityId = cte.UniversityId then 3 else 0 end  +
case when ds.Country = cte.Country then 2 else 0 end +
cast(1 as float)/ISNULL(nullif( DATEDIFF(minute, ds.[DateTime], GETUTCDATE()   ),0),1) desc
OFFSET :page*50 ROWS
FETCH NEXT 50 ROWS ONLY";

                //this is because we don't want to aggregate all the historical data


                const string filter = @"select distinct top 50 [Type]
                from sb.Document ds 	where 
     (:typeFilterCount = 0  or ds.Type in (:typefilter))";


                var sqlQuery = _dapperRepository.CreateSQLQuery(sql);
                sqlQuery.SetInt32("page", query.Page);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetString("country", query.Country);
                sqlQuery.SetInt32("typeFilterCount", query.Filter?.Length ?? 0);
                sqlQuery.SetParameterList("typefilter", query.Filter ?? Enumerable.Repeat("x",1));
                

                sqlQuery.SetResultTransformer(new DeepTransformer<DocumentFeedDto>('_'));
                var future = sqlQuery.Future<DocumentFeedDto>();


                var filterQuery = _dapperRepository.CreateSQLQuery(filter);
                filterQuery.SetInt32("typeFilterCount", query.Filter?.Length ?? 0);
                filterQuery.SetParameterList("typefilter", query.Filter ?? Enumerable.Repeat("x", 1));
                var filtersFuture = filterQuery.Future<string>();


                var filters = await filtersFuture.GetEnumerableAsync(token);
                var list = await future.GetEnumerableAsync(token);
                return new DocumentFeedWithFacetDto()
                {
                    Facet = filters,
                    Result = list
                };
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