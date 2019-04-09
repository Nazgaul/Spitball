using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Query.Stuff;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Transform;

namespace Cloudents.Query.Questions
{
    public class QuestionAggregateQuery: IQuery<QuestionFeedWithFacetDto>
    {
        public QuestionAggregateQuery(long userId, int page, string filter = null)
        {
            Page = page;
            UserId = userId;
            Filter = filter;
        }

        private int Page { get; }
        private long UserId { get; }
        private string Filter { get; }

        internal sealed class QuestionAggregateQueryHandler : IQueryHandler<QuestionAggregateQuery, QuestionFeedWithFacetDto>
        {
            private readonly IStatelessSession _repository;

            public QuestionAggregateQueryHandler(QuerySession repository)
            {
                _repository = repository.StatelessSession;
            }

            public async Task<QuestionFeedWithFacetDto> GetAsync(QuestionAggregateQuery query, CancellationToken token)
            {
                const string sql = @"with cte as (
			select u2.Id as UniversityId, 
			COALESCE(u2.country,u.country) as CountryId, u.id as userid
			 from sb.[user] u 
			 left join sb.University u2 on u.UniversityId2 = u2.Id
			 where u.id = :userid 
				) 
select q.Id as Id,
  isnull(q.Subject_id,0) as [Subject],
  q.Price as Price,
   q.Text as [Text],
    q.Attachments as Files,
	 q.CourseId as Course,
(SELECT count(*) as answers FROM sb.[Answer] a WHERE a.QuestionId = q.Id and a.State = 'Ok') as Answers, 
q.Updated as [DateTime],
case when not (q.CorrectAnswer_id is null) then 1 else 0 end as HasCorrectAnswer,
q.Language as CultureInfo,
u.Id as User_Id,
u.Name as User_Name,
u.Score as User_Score,
u.Image as User_Image,
q.VoteCount as Vote_Votes
from sb.Question q
join sb.[user] u
	on u.id = q.UserId
left join sb.University un on un.Id = q.UniversityId
,cte
where  q.CourseId in (select courseId from sb.usersCourses where userid = cte.userid) 
    and (:typefilter is null or q.Subject_id = :typefilter) and q.State = 'Ok'
order by case when q.UniversityId = cte.UniversityId then 3 else 0 end  +
case when un.Country = cte.CountryId then 2 else 0 end +
cast(1 as float)/case when DATEDIFF(DAY, q.Updated, GETUTCDATE()) = 0 then 1 else DATEDIFF(DAY, q.Updated, GETUTCDATE()) end desc
OFFSET :page*50 ROWS
FETCH NEXT 50 ROWS ONLY;";


                const string filter = @"select distinct Subject_id
                from sb.Question q
                    where q.CourseId in (
                            select courseid from sb.userscourses where userid = :userid 
                                        )";


                var sqlQuery = _repository.CreateSQLQuery(sql);
                sqlQuery.SetInt32("page", query.Page);
                sqlQuery.SetInt64("userid", query.UserId);
                sqlQuery.SetString("typefilter", query.Filter);
                sqlQuery.SetResultTransformer(new DeepTransformer<QuestionFeedDto>(
                    '_', new CustomDeepTransformer<QuestionFeedDto>()));
                var future = sqlQuery.Future<QuestionFeedDto>();


                var filterQuery = _repository.CreateSQLQuery(filter);
                filterQuery.SetInt64("userid", query.UserId);
                var filtersFuture = filterQuery
                    .Future<int?>();


                var filters = await filtersFuture.GetEnumerableAsync(token);
                var list = await future.GetEnumerableAsync(token);
                return new QuestionFeedWithFacetDto()
                {
                    //Facet = (IEnumerable<QuestionSubject?>)filters,
                    Result = list
                };
            }
        }
    }
}
