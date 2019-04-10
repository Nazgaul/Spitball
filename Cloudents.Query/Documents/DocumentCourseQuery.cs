using Cloudents.Core.DTOs;
using Cloudents.Query.Stuff;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
    public class DocumentCourseQuery : IQuery<DocumentFeedWithFacetDto>
    {
        public DocumentCourseQuery(long userId, int page, string course, string[] filter)
        {
            Page = page;
            UserId = userId;
            Course = course;
            Filter = filter;
        }

        private int Page { get; }
        private long UserId { get;  }
        private string Course { get;  }
        private string[] Filter { get; }
        

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
,(select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = cte.userid) as Vote_Vote
,d.Price as Price

from sb.Document d 
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId,
cte
where d.CourseName = :course and (:typefilter is null or d.Type in :typefilter)
order by case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
case when un.Country = cte.Country then 2 else 0 end +
cast(1 as float)/DATEDIFF(day, d.updateTime, GETUTCDATE()) desc
OFFSET :page*50 ROWS
FETCH NEXT 50 ROWS ONLY";


                const string filter = @"select distinct [Type]
                from sb.Document d
                    where d.CourseName = :course";


                var sqlQuery = _repository.CreateSQLQuery(sql);
                sqlQuery.SetInt32("page", query.Page);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetString("course", query.Course);
                sqlQuery.SetParameterList("typefilter", query.Filter);
                sqlQuery.SetResultTransformer(new DeepTransformer<DocumentFeedDto>('_'));
                var future = sqlQuery.Future<DocumentFeedDto>();


                var filterQuery = _repository.CreateSQLQuery(filter);
                filterQuery.SetString("course", query.Course); 
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
