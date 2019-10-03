using System.Linq;
using Cloudents.Core.DTOs;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    public class DocumentCourseQuery : IQuery<DocumentFeedWithFacetDto>
    {
        public DocumentCourseQuery(long userId, int page, string course, string[] filter, string country)
        {
            Page = page;
            UserId = userId;
            Course = course;
            Filter = filter;
            Country = country;
        }

        private int Page { get; }
        private long UserId { get;  }
        private string Course { get;  }
        private string[] Filter { get; }
        private string Country { get; }


        internal sealed class DocumentAggregateQueryHandler : IQueryHandler<DocumentCourseQuery, DocumentFeedWithFacetDto>
        {
            private readonly IStatelessSession _repository;

            public DocumentAggregateQueryHandler(QuerySession repository)
            {
                _repository = repository.StatelessSession;
            }


            public async Task<DocumentFeedWithFacetDto> GetAsync(DocumentCourseQuery query, CancellationToken token)
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
	,ds.Title
	,ds.User_Id
	,ds.User_Name
	,ds.User_Image
	,ds.[Views]
	,ds.Downloads
	,ds.[DateTime]
	,ds.Vote_Votes
	,(select v.VoteType from sb.Vote v where v.DocumentId = ds.Id and v.UserId = cte.userid) as Vote_Vote
	,ds.Price
    ,ds.Purchased
,ds.DocumentType as documentType
,ds.duration as Duration
from sb.iv_DocumentSearch ds
,cte
where ds.Course = :course
 and (:typeFilterCount = 0 or ds.Type in (:typefilter))
order by case when ds.UniversityId = cte.UniversityId then 3 else 0 end  +
case when ds.Country = cte.Country then 2 else 0 end +
cast(1 as float)/ISNULL(nullif( DATEDIFF(minute, ds.[DateTime], GETUTCDATE()   ),0),1) desc
OFFSET :page*20 ROWS
FETCH NEXT 20 ROWS ONLY";


                const string filter = @"select distinct [Type]
                from sb.Document d
                    where d.CourseName = :course
and (:typeFilterCount = 0 or d.Type in (:typefilter))";


                var sqlQuery = _repository.CreateSQLQuery(sql);
                sqlQuery.SetInt32("page", query.Page);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetString("course", query.Course);
                sqlQuery.SetString("country", query.Country);
                sqlQuery.SetInt32("typeFilterCount", query.Filter?.Length ?? 0);
                sqlQuery.SetParameterList("typefilter", query.Filter ?? Enumerable.Repeat("x", 1));
                sqlQuery.SetResultTransformer(new DeepTransformer<DocumentFeedDto>('_', new SbAliasToBeanResultTransformer<DocumentFeedDto>()) );
                var future = sqlQuery.Future<DocumentFeedDto>();


                var filterQuery = _repository.CreateSQLQuery(filter);
                filterQuery.SetString("course", query.Course);
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
            }
        }
    }
}
