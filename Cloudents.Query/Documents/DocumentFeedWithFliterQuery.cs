using Cloudents.Core.DTOs;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Query.Documents
{
   public class DocumentFeedWithFliterQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public DocumentFeedWithFliterQuery(int page, long userId, FeedType? filter, string country, string course, int pageSize)
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
        private FeedType? Filter { get; }

        private string Country { get; }

        private string Course { get; }
        public int PageSize { get; }

        internal sealed class DocumentFeedWithFliterQueryHandler : IQueryHandler<DocumentFeedWithFliterQuery, IEnumerable<DocumentFeedDto>>
        {

            private readonly IDapperRepository _dapperRepository;
            private readonly IJsonSerializer _jsonSerializer;

            public DocumentFeedWithFliterQueryHandler(IDapperRepository dapperRepository, IJsonSerializer jsonSerializer)
            {
                _dapperRepository = dapperRepository;
                _jsonSerializer = jsonSerializer;
            }

            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(DocumentFeedWithFliterQuery query, CancellationToken token)
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


select 'd' as type
, d.CourseName as Course
, d.UniversityId as UniversityId
, d.UpdateTime as DateTime
, (select d.Id
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
and d.DocumentType = @documentType
order by
case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
cast(1 as float)/ISNULL(nullif(DATEDiff(minute, d.UpdateTime, GetUtcDATE()   ),0),1) desc
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY;";
                const string sqlWithoutCourse = @"
with cte as (
select top 1 * from (select 1 as o, u2.Id as UniversityId, COALESCE(u2.country,u.country) as Country, u.id as userid
 from sb.[user] u
 left join sb.University u2 on u.UniversityId2 = u2.Id
 where u.id = @userId
 union
 select 2,null,@country,0) t
 order by o
)

select 'd' as type
,d.CourseName as Course
,d.UniversityId as UniversityId
,d.UpdateTime as DateTime
,(select d.Id
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
and d.DocumentType = @documentType
order by
case when d.CourseName in (select courseId from sb.usersCourses where userid = cte.userid) then 4 else 0 end +
case when d.UniversityId = cte.UniversityId then 3 else 0 end  +
cast(1 as float)/ISNULL(nullif( DATEDIFF(minute, d.UpdateTime, GETUTCDATE()   ),0),1) desc
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";

                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;

                var result = new List<DocumentFeedDto>();
                using (var conn = _dapperRepository.OpenConnection())
                using (var reader = await conn.ExecuteReaderAsync(sql, new
                {
                    query.Page,
                    query.UserId,
                    query.Country,
                    query.Course,
                    query.PageSize,
                    documentType = query.Filter.ToString()

                }))
                {
                    if (reader.Read())
                    {
                        var col = reader.GetOrdinal("type");
                        var colJson = reader.GetOrdinal("JsonArray");
                        do
                        {
                            var v = reader.GetString(colJson);
                            var documents = _jsonSerializer.Deserialize<IEnumerable<DocumentFeedDto>>(v, JsonConverterTypes.TimeSpan);
                            result.Add(documents.First());
                        } while (reader.Read());
                    }
                }

                return result;
            }
        }
    }
}
