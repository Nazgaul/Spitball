using System;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.DTOs.Feed;
using Cloudents.Core.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Documents
{
    public class FeedAggregateQuery : IQuery<IEnumerable<FeedDto>>
    {
        public FeedAggregateQuery(long userId, int page, Country country, string? course, int pageSize)
        {
            Page = page;
            UserId = userId;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            if (!string.IsNullOrEmpty(course))
            {
                Course = course;
            }

            PageSize = pageSize;
        }

        private int Page { get; }

        private long UserId { get; }
        

        private Country Country { get; }

        private string? Course { get; }
        private int PageSize { get; }

        internal sealed class DocumentAggregateQueryHandler : IQueryHandler<FeedAggregateQuery, IEnumerable<FeedDto>>
        {



            private readonly IDapperRepository _dapperRepository;
            private readonly IJsonSerializer _jsonSerializer;
            private readonly IUrlBuilder _urlBuilder;

            public DocumentAggregateQueryHandler(IDapperRepository dapperRepository, IJsonSerializer jsonSerializer, IUrlBuilder urlBuilder)
            {
                _dapperRepository = dapperRepository;
                _jsonSerializer = jsonSerializer;
                _urlBuilder = urlBuilder;
            }

            // If you chnage enything in the sql query tou need to take care to 
            // QuestionFeedWithFliterQuery and DocumentFeedWithFilterQuery as well
            public async Task<IEnumerable<FeedDto>> GetAsync(FeedAggregateQuery query, CancellationToken token)
            {
                const string sqlWithCourse = @"
select R.*
from
(
select 'd' as type
, d.CourseName as Course
, d.UpdateTime as DateTime
, (select d.Id--id,
, d.Price
, d.CourseName as Course
, d.UpdateTime as DateTime
, d.Language as CultureInfo
, u.Id as 'User.Id'
, u.Name as 'User.Name'
, u.ImageName as 'User.Image'
, COALESCE(d.description,metaContent) as Snippet
, d.Name as Title
, d.[Views]
,d.PriceType as PriceType
, d.Downloads
, d.VoteCount as 'Vote.Votes'
, (select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = @userId) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.DocumentType as documentType for json path) as JsonArray,
case when d.DocumentType = 'Video' then 1 else 0 end as IsVideo,
case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
from sb.document d
join sb.[user] u on d.UserId = u.Id
where u.sbCountry = @Country
and d.State = 'Ok'
and d.courseName = @course

union all

SELECT  'q' as type
,q.CourseId as Course

,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
q.CourseId as Course,
(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE(this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
q.Updated as DateTime,
q.Language as CultureInfo
--TODO from cross join
,x.Id as 'FirstAnswer.User.Id'
,x.ImageName as 'FirstAnswer.User.Image'
,x.Name as 'FirstAnswer.User.Name'
,x.Text as 'FirstAnswer.Text'
,x.Created as 'FirstAnswer.DateTime'
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.ImageName as 'User.Image'
,'Question' as documentType for json path) JsonArray,
0 as IsVideo,
case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id

outer apply (
select top 1 text, u.id, u.name, u.ImageName, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created
) as x

where
 u.sbCountry = @Country
and q.courseId = @course

and q.State = 'Ok'
  ) R

order by
DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())  +
DATEDiff(hour, R.DateTime, GetUtcDATE()) +
case when r.IsVideo = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end + 
case when r.IsFollow = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";
                const string sqlWithoutCourse = @"

select R.*
from
(
select 'd' as type
,d.CourseName as Course
,d.UpdateTime as DateTime
,(select d.Id --id,
,d.Price
,d.CourseName as Course
,d.PriceType as PriceType
,d.UpdateTime as DateTime
,d.Language as CultureInfo
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.ImageName as 'User.Image'
,COALESCE(d.description,metaContent) as Snippet
,d.Name as Title
,d.[Views]
,d.Downloads
,d.VoteCount as  'Vote.Votes'
,(select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = @userId) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.DocumentType as documentType for json path) as JsonArray,
case when d.DocumentType = 'Video' then 1 else 0 end as IsVideo,
case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
from sb.document d
join sb.[user] u on d.UserId = u.Id


where
 u.SbCountry = @Country 
and d.UpdateTime > GETUTCDATE() - 182
and d.State = 'Ok'
and (d.CourseName in (select courseId from sb.usersCourses where userid = @userId) or @userid <= 0)

union all

SELECT  'q' as type
,q.CourseId as Course
,q.Updated as DateTime
,(select q.Id as Id,
q.Text as Text,
q.CourseId as Course,
(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE (this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
q.Updated as DateTime,
q.Language as CultureInfo
--TODO from cross join
,x.Id as 'FirstAnswer.User.Id'
,x.ImageName as 'FirstAnswer.User.Image'
,x.Name as 'FirstAnswer.User.Name'
,x.Text as 'FirstAnswer.Text'
,x.Created as 'FirstAnswer.DateTime'
,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.ImageName as 'User.Image'
,'Question' as documentType for json path) JsonArray
, 0 as IsVideo,
case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
FROM sb.[Question] q
join sb.[user] u
	on q.UserId = u.Id
outer apply (
select  top 1 text,u.id,u.name,u.ImageName, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
where a.QuestionId = q.Id and state = 'Ok' order by a.created

) as x
where
 u.SbCountry = @Country 
and q.Updated > GETUTCDATE() - 182
and q.State = 'Ok'
and (q.CourseId in (select courseId from sb.usersCourses where userid = @userId) or @userid <= 0)
  ) R
  
order by
case when R.Course in (select courseId from sb.usersCourses where userid = @userId) then 0 else DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())*2 end +
DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())  +
DATEDiff(hour, R.DateTime, GetUtcDATE()) +
case when r.IsVideo = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end + 
case when r.IsFollow = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";



                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;
                //this is because we don't want to aggregate all the historical data
                var result = new List<FeedDto>();
                using var conn = _dapperRepository.OpenConnection();
                using var reader = await conn.ExecuteReaderAsync(sql, new
                {
                    query.Page,
                    query.UserId,
                    Country = query.Country.Id,
                    query.Course,
                    query.PageSize

                });
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
                                var question = _jsonSerializer.Deserialize<IEnumerable<QuestionFeedDto>>(v).First();
                                if (question.User.Image != null)
                                {
                                    question.User.Image =
                                        _urlBuilder.BuildUserImageEndpoint(question.User.Id, question.User.Image);
                                }

                                if (question.FirstAnswer?.User.Image != null)
                                {
                                    question.FirstAnswer.User.Image = _urlBuilder.BuildUserImageEndpoint(question.FirstAnswer.User.Id, question.FirstAnswer.User.Image);
                                }

                                result.Add(question);
                                break;
                            case "d":
                                var document = _jsonSerializer.Deserialize<IEnumerable<DocumentFeedDto>>(v, JsonConverterTypes.TimeSpan).First();
                                if (document.User.Image != null)
                                {
                                    document.User.Image =
                                        _urlBuilder.BuildUserImageEndpoint(document.User.Id, document.User.Image);
                                }

                                result.Add(document);
                                break;
                        }
                    } while (reader.Read());
                }

                return result;
            }
        }
    }

}


