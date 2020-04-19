using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Questions
{
    public class QuestionFeedWithFilterQuery : IQuery<IEnumerable<QuestionFeedDto>>
    {
        public QuestionFeedWithFilterQuery(int page, long userId, string country, string course, int pageSize)
        {
            Page = page;
            UserId = userId;
            Country = country;
            Course = course;
            PageSize = pageSize;
        }
        private int Page { get; }

        private long UserId { get; }

        private string Country { get; }

        private string Course { get; }
        private int PageSize { get; }

        internal sealed class DocumentFeedWithFilterQueryHandler : IQueryHandler<QuestionFeedWithFilterQuery, IEnumerable<QuestionFeedDto>>
        {
            private readonly IDapperRepository _dapperRepository;
            private readonly IJsonSerializer _jsonSerializer;

            public DocumentFeedWithFilterQueryHandler(IDapperRepository dapperRepository, IJsonSerializer jsonSerializer)
            {
                _dapperRepository = dapperRepository;
                _jsonSerializer = jsonSerializer;
            }

            // If you chnage enything in the sql query tou need to take care to 
            // FeedAggregateQuery and DocumentFeedWithFilterQuery as well
            public async Task<IEnumerable<QuestionFeedDto>> GetAsync(QuestionFeedWithFilterQuery query, CancellationToken token)
            {
                const string sqlWithCourse = @"with cte as (
select top 1 * from(select 1 as o, u.country as Country, u.id as userid
 from sb.[user] u
 where u.id = @userid
 union
 select 2, @country, 0) t
    order by o
)
SELECT  'q' as type
,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
(select cardDisplay from sb.course2 where id = q.CourseId2) as Course,

(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE(this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
q.Updated as DateTime,
q.Language as CultureInfo
--TODO from cross join
,x.Id as 'FirstAnswer.User.Id'
,x.Image as 'FirstAnswer.User.Image'
,x.Name as 'FirstAnswer.User.Name'
,x.Text as 'FirstAnswer.Text'
,x.Created as 'FirstAnswer.DateTime'
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.Image as 'User.Image'
,'Question' as documentType for json path) JsonArray
,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id

outer apply (
select top 1 text, u.id, u.name, u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created
) as x
join cte on u.country = cte.country
where
    q.Updated > GetUtcDATE() - 182
and q.courseId2 = (select id from sb.Course2 where SearchDisplay = @course)
and q.State = 'Ok'
order by
DATEDiff(hour, q.Updated, GetUtcDATE()) +
case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";

                const string sqlWithoutCourse = @"with cte as (
select top 1 * from (select 1 as o, u.country as Country, u.id as userid
 from sb.[user] u
 where u.id = @userid
 union
 select 2,@country,0) t
 order by o
)


SELECT  'q' as type
,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
(select cardDisplay from sb.course2 where id = q.CourseId2) as Course,
(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE (this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
q.Updated as DateTime,
q.Language as CultureInfo
--TODO from cross join
,x.Id as 'FirstAnswer.User.Id'
,x.Image as 'FirstAnswer.User.Image'
,x.Name as 'FirstAnswer.User.Name'
,x.Text as 'FirstAnswer.Text'
,x.Created as 'FirstAnswer.DateTime'
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.Image as 'User.Image'
,'Question' as documentType for json path) JsonArray
,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id

outer apply (
select  top 1 text,u.id,u.name,u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created

) as x
join cte on u.country = cte.country
where 
    q.Updated > GETUTCDATE() - 182
and q.State = 'Ok'
and (q.CourseId2 in (select courseId from sb.userCourse2 where userid = cte.userid) or @userid <= 0)
order by
DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())  +
DATEDiff(hour, q.Updated, GetUtcDATE()) +
case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";

                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;

                var result = new List<QuestionFeedDto>();
                using var conn = _dapperRepository.OpenConnection();
                using var reader = await conn.ExecuteReaderAsync(sql, new
                {
                    query.Page,
                    query.UserId,
                    query.Country,
                    query.Course,
                    query.PageSize

                });
                if (reader.Read())
                {
                    var colJson = reader.GetOrdinal("JsonArray");
                    do
                    {
                        var v = reader.GetString(colJson);
                        var questions = _jsonSerializer.Deserialize<IEnumerable<QuestionFeedDto>>(v);
                        result.Add(questions.First());
                            
                    } while (reader.Read());
                }

                return result;
            }
        }
    }
}
