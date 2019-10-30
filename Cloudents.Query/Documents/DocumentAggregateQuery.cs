using System.Collections.Generic;
using Cloudents.Core.DTOs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Cloudents.Core.Interfaces;

namespace Cloudents.Query.Documents
{
    public class FeedAggregateQuery : IQuery<IEnumerable<FeedDto>>
    {
        public FeedAggregateQuery(long userId, int page, string[] filter, string country, string course, int pageSize)
        {
            Page = page;
            UserId = userId;
            Filter = filter;
            Country = country;
            Course = course;
            PageSize = pageSize;
        }

        private int Page { get; }

        private long UserId { get; }
        private string[] Filter { get; }

        private string Country { get; }

        private string Course { get; }
        public int PageSize { get;}

        internal sealed class DocumentAggregateQueryHandler : IQueryHandler<FeedAggregateQuery, IEnumerable<FeedDto>>
        {



            private readonly IDapperRepository _dapperRepository;
            private readonly IJsonSerializer _jsonSerializer;

            public DocumentAggregateQueryHandler(IDapperRepository dapperRepository, IJsonSerializer jsonSerializer)
            {
                _dapperRepository = dapperRepository;
                _jsonSerializer = jsonSerializer;
            }


            public async Task<IEnumerable<FeedDto>> GetAsync(FeedAggregateQuery query, CancellationToken token)
            {
                const string sqlWithCourse = @"with cte as (
select top 1 * from(select 1 as o, u2.Id as UniversityId, COALESCE(u2.country, u.country) as Country, u.id as userid
 from sb.[user] u
 left
 join sb.University u2 on u.UniversityId2 = u2.Id
 where u.id = @userid
 union
 select 2, null, @country, 0) t
    order by o
)

select R.*
from
(
select 'd' as type
, d.CourseName as Course
, d.UniversityId as UniversityId
, d.UpdateTime as DateTime
, (select d.Id--id,
, d.Price
, d.CourseName as Course
, d.UpdateTime as DateTime
, d.Language as CultureInfo
, un.Name as University
, u.Id as 'User.Id'
, u.Name as 'User.Name'
, u.Image as 'User.Image'
, un.Id as UniversityId
, COALESCE(d.description,metaContent) as Snippet
, d.Name as Title
, d.[Views]
, d.Downloads
, d.VoteCount as 'Vote.Votes'
, (select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = cte.userid) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.DocumentType as documentType for json path) as JsonArray
from sb.document d
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId
,cte
where
    d.UpdateTime > GETUTCDATE() - 182


and un.country = cte.country
and d.State = 'Ok'
and d.courseName = @course

union all

SELECT  'q' as type
,q.CourseId as Course
,q.UniversityId as UniversityId
,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
q.CourseId as Course,
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

FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id
join sb.University un on q.UniversityId = un.Id
outer apply (
select top 1 text, u.id, u.name, u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created

) as x
,cte

where
    q.Updated > GetUtcDATE() - 182
and un.country = cte.country
and q.courseId = @course

and q.State = 'Ok'
  ) R,
  cte
order by
case when R.UniversityId = cte.UniversityId then 3 else 0 end  +
cast(1 as float)/ISNULL(nullif(DATEDiff(minute, R.DateTime, GetUtcDATE()   ),0),1) desc
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";
                const string sqlWithoutCourse = @"with cte as (
select top 1 * from (select 1 as o, u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
 from sb.[user] u
 left join sb.University u2 on u.UniversityId2 = u2.Id
 where u.id = @userid
 union
 select 2,null,@country,0) t
 order by o
)

select R.*
from
(
select 'd' as type
,d.CourseName as Course
,d.UniversityId as UniversityId
,d.UpdateTime as DateTime
,(select d.Id --id,
,d.Price
,d.CourseName as Course
,d.UpdateTime as DateTime
,d.Language as CultureInfo
,un.Name as University
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.Image as 'User.Image'
,un.Id as UniversityId
,COALESCE(d.description,metaContent) as Snippet
,d.Name as Title
,d.[Views]
,d.Downloads
,d.VoteCount as  'Vote.Votes'
,(select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = cte.userid) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.DocumentType as documentType for json path) as JsonArray
from sb.document d
join sb.[user] u on d.UserId = u.Id
join sb.University un on un.Id = d.UniversityId
,cte
where
    d.UpdateTime > GETUTCDATE() - 182
and un.country = cte.country
and d.State = 'Ok'

union all

SELECT  'q' as type
,q.CourseId as Course
,q.UniversityId as UniversityId
,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
q.CourseId as Course,
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

FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id
join sb.University un on q.UniversityId = un.Id
outer apply (
select  top 1 text,u.id,u.name,u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created

) as x
,cte

where
    q.Updated > GETUTCDATE() - 182
and un.country = cte.country

and q.State = 'Ok'
  ) R,
  cte
order by
case when R.Course in (select courseId from sb.usersCourses where userid = cte.userid) then 4 else 0 end +
case when R.UniversityId = cte.UniversityId then 3 else 0 end  +
cast(1 as float)/ISNULL(nullif( DATEDIFF(minute, R.DateTime, GETUTCDATE()   ),0),1) desc
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";


                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;
                //this is because we don't want to aggregate all the historical data
                var result = new List<FeedDto>();
                using (var conn = _dapperRepository.OpenConnection())
                using (var reader = await conn.ExecuteReaderAsync(sql, new
                {
                    query.Page,
                    query.UserId,
                    query.Country,
                    query.Course,
                    query.PageSize

                }))
                {
                    if (reader.Read())
                    {
                        var col = reader.GetOrdinal("type");
                        var colJson = reader.GetOrdinal("JsonArray");
                        do
                        {
                            var v = reader.GetString(colJson);
                            switch (reader.GetString(col))
                            {

                                case "q":
                                    var questions = _jsonSerializer.Deserialize<IEnumerable<QuestionFeedDto>>(v);
                                    result.Add(questions.First());
                                    break;
                                case "d":
                                    var documents = _jsonSerializer.Deserialize<IEnumerable<DocumentFeedDto>>(v, JsonConverterTypes.TimeSpan);
                                    result.Add(documents.First());
                                    break;
                            }
                        } while (reader.Read());
                    }
                }

                return result;
            }
        }
    }

}


