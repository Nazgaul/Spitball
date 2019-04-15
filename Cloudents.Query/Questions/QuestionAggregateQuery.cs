//using Cloudents.Core.DTOs;
//using Cloudents.Query.Stuff;
//using NHibernate;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Questions
//{
//    public class QuestionAggregateQuery: IQuery<QuestionFeedWithFacetDto>
//    {
//        public QuestionAggregateQuery(long userId, int page, string[] filter)
//        {
//            Page = page;
//            UserId = userId;
//            Filter = filter;
//        }

//        private int Page { get; }
//        private long UserId { get; }
//        private string[] Filter { get; }

//        internal sealed class QuestionAggregateQueryHandler : IQueryHandler<QuestionAggregateQuery, QuestionFeedWithFacetDto>
//        {
//            private readonly IStatelessSession _repository;

//            public QuestionAggregateQueryHandler(QuerySession repository)
//            {
//                _repository = repository.StatelessSession;
//            }

//            public async Task<QuestionFeedWithFacetDto> GetAsync(QuestionAggregateQuery query, CancellationToken token)
//            {
//                const string sql = @"with cte as (
//			select u2.Id as UniversityId, 
//			COALESCE(u2.country,u.country) as CountryId, u.id as userid
//			 from sb.[user] u 
//			 left join sb.University u2 on u.UniversityId2 = u2.Id
//			 where u.id = :userid 
//				) 
//select  qs.Id as Id,
//	qs.[Subject],
//	qs.Price,
//	qs.[Text],
//	qs.Files,
//	qs.Course,
//	(SELECT count(*) as answers FROM sb.[Answer] a WHERE a.QuestionId = qs.Id and a.State = 'Ok') as Answers, 
//	qs.[DateTime],
//	qs.HasCorrectAnswer,
//	qs.CultureInfo,
//	qs.User_Id,
//	qs.User_Name,
//	qs.User_Score,
//	qs. User_Image,
//	qs.Vote_Votes 
//from sb.iv_QuestionSearch qs
//left join sb.University un on un.Id = qs.UniversityId
//,cte
//where  qs.Course in (select courseId from sb.usersCourses where userid = cte.userid) 
//    and (:typeFilterCount = 0 or qs.Type in (:typefilter))
//    and q.State = 'Ok'
//order by case when qs.UniversityId = cte.UniversityId then 3 else 0 end  +
//case when un.Country = cte.CountryId then 2 else 0 end +
//cast(1 as float)/case when DATEDIFF(DAY, qs.[DateTime], GETUTCDATE()) = 0 then 1 else DATEDIFF(DAY, qs.[DateTime], GETUTCDATE()) end desc
//OFFSET :page*50 ROWS
//FETCH NEXT 50 ROWS ONLY;";


//                const string filter = @"select distinct Subject_id
//                from sb.Question q
//                    where q.CourseId in (
//                            select courseid from sb.userscourses where userid = :userid 
//                                        )";


//                var sqlQuery = _repository.CreateSQLQuery(sql);
//                sqlQuery.SetInt32("page", query.Page);
//                sqlQuery.SetInt64("userid", query.UserId);
//                sqlQuery.SetInt32("typeFilterCount", query.Filter?.Length ?? 0);
//                sqlQuery.SetParameterList("typefilter", query.Filter ?? Enumerable.Repeat("x", 1));
//                sqlQuery.SetResultTransformer(new DeepTransformer<QuestionFeedDto>(
//                    '_', new CustomDeepTransformer<QuestionFeedDto>()));
//                var future = sqlQuery.Future<QuestionFeedDto>();


//                var filterQuery = _repository.CreateSQLQuery(filter);
//                filterQuery.SetInt64("userid", query.UserId);
//                var filtersFuture = filterQuery
//                    .Future<int?>();


//                var filters = await filtersFuture.GetEnumerableAsync(token);
//                var list = await future.GetEnumerableAsync(token);
//                return new QuestionFeedWithFacetDto()
//                {
//                    //Facet = (IEnumerable<QuestionSubject?>)filters,
//                    Result = list
//                };
//            }
//        }
//    }
//}
