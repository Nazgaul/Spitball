﻿using System;
using Cloudents.Core.DTOs.Documents;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Query.Documents
{
    public class DocumentFeedWithFilterQuery : IQuery<IEnumerable<DocumentFeedDto>>
    {
        public DocumentFeedWithFilterQuery(int page, long userId, FeedType? filter,
            Country country, string course, int pageSize)
        {
            Page = page;
            UserId = userId;
            Filter = filter;
            Country = country ?? throw new ArgumentNullException(nameof(country));
            Course = course;
            PageSize = pageSize;
        }
        private int Page { get; }

        private long UserId { get; }
        private FeedType? Filter { get; }

        private Country Country { get; }

        private string Course { get; }
        private int PageSize { get; }

        internal sealed class DocumentFeedWithFilterQueryHandler : IQueryHandler<DocumentFeedWithFilterQuery, IEnumerable<DocumentFeedDto>>
        {

            private readonly IDapperRepository _dapperRepository;
            private readonly IJsonSerializer _jsonSerializer;

            public DocumentFeedWithFilterQueryHandler(IDapperRepository dapperRepository,
                IJsonSerializer jsonSerializer)
            {
                _dapperRepository = dapperRepository;
                _jsonSerializer = jsonSerializer;
            }

            // If you change everything in the sql query tou need to take care to 
            // QuestionFeedWithFilterQuery and FeedAggregateQuery as well
            public async Task<IEnumerable<DocumentFeedDto>> GetAsync(DocumentFeedWithFilterQuery query, CancellationToken token)
            {
                const string sqlWithCourse = @"
select 'd' as type
, d.CourseName as Course
, d.UpdateTime as DateTime
, (select d.Id
, d.Price
, d.CourseName as Course
, d.UpdateTime as DateTime
, d.Language as CultureInfo
, u.Id as 'User.Id'
, u.Name as 'User.Name'
, u.Image as 'User.Image'
, COALESCE(d.description,metaContent) as Snippet
, d.Name as Title
, d.[Views]
, d.Downloads
, d.PriceType as PriceType
, d.VoteCount as 'Vote.Votes'
, (select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = @userId) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.DocumentType as documentType for json path) as JsonArray
,case when d.DocumentType = 'Video' then 1 else 0 end as IsVideo
,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
from sb.document d
join sb.[user] u on d.UserId = u.Id

where
d.State = 'Ok'
and u.sbCountry = @country
and d.courseName = @course
and COALESCE(d.DocumentType,'Document') = @documentType
order by
DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE()) +
DATEDiff(hour, d.UpdateTime, GetUtcDATE()) +
case when case when d.DocumentType = 'Video' then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end + 
case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY;";
                const string sqlWithoutCourse = @"

select 'd' as type
,d.CourseName as Course

,d.UpdateTime as DateTime
,(select d.Id
,d.Price
,d.CourseName as Course
,d.UpdateTime as DateTime
,d.Language as CultureInfo

,u.Id as 'User.Id'
,u.Name as 'User.Name'
,u.Image as 'User.Image'

,COALESCE(d.description,metaContent) as Snippet
,d.Name as Title
,d.[Views]
,d.Downloads
,d.VoteCount as  'Vote.Votes'
,(select v.VoteType from sb.Vote v where v.DocumentId = d.Id and v.UserId = @userId) as 'Vote.Vote'
,(select count(1) from sb.[Transaction] where DocumentId = d.Id and [Action] = 'SoldDocument') as Purchased
,d.duration as Duration
,d.PriceType as PriceType
,d.DocumentType as documentType for json path) as JsonArray
,case when d.DocumentType = 'Video' then 1 else 0 end as IsVideo
,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
from sb.document d
join sb.[user] u on d.UserId = u.Id
where
    d.UpdateTime > GetUtcDate() - 182
and u.SbCountry = @country
and d.State = 'Ok'
and COALESCE(d.DocumentType,'Document') = @documentType
and (d.CourseName in (select courseId from sb.usersCourses where userid = @userId) or @userid <= 0)
order by
DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())  +
DATEDiff(hour, d.UpdateTime, GetUtcDATE()) +
case when case when d.DocumentType = 'Video' then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end + 
case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
OFFSET @page*@pageSize ROWS
FETCH NEXT @pageSize ROWS ONLY";

                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;

                var result = new List<DocumentFeedDto>();
                using var conn = _dapperRepository.OpenConnection();
                using var reader = await conn.ExecuteReaderAsync(sql, new
                {
                    query.Page,
                    query.UserId,
                    Country = query.Country.Id,
                    query.Course,
                    query.PageSize,
                    documentType = query.Filter.ToString()

                });
                if (reader.Read())
                {
                    var colJson = reader.GetOrdinal("JsonArray");
                    do
                    {
                        var v = reader.GetString(colJson);
                        var documents = _jsonSerializer.Deserialize<IEnumerable<DocumentFeedDto>>(v, JsonConverterTypes.TimeSpan);
                        result.Add(documents.First());
                    } while (reader.Read());
                }

                return result;
            }
        }
    }
}
