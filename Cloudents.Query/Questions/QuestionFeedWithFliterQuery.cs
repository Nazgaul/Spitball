//using System;
//using Cloudents.Core.DTOs.Feed;
//using Cloudents.Core.Interfaces;
//using Dapper;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.Entities;

//namespace Cloudents.Query.Questions
//{
//    public class QuestionFeedWithFilterQuery : IQuery<IEnumerable<QuestionFeedDto>>
//    {
//        public QuestionFeedWithFilterQuery(int page, long userId, Country country, string? course, int pageSize)
//        {
//            Page = page;
//            UserId = userId;
//            Country = country ?? throw new ArgumentNullException(nameof(country));
//            Course = course;
//            PageSize = pageSize;
//        }
//        private int Page { get; }

//        private long UserId { get; }

//        private Country Country { get; }

//        private string? Course { get; }
//        private int PageSize { get; }

//        internal sealed class DocumentFeedWithFilterQueryHandler : IQueryHandler<QuestionFeedWithFilterQuery, IEnumerable<QuestionFeedDto>>
//        {
//            private readonly IDapperRepository _dapperRepository;
//            private readonly IJsonSerializer _jsonSerializer;

//            public DocumentFeedWithFilterQueryHandler(IDapperRepository dapperRepository, IJsonSerializer jsonSerializer)
//            {
//                _dapperRepository = dapperRepository;
//                _jsonSerializer = jsonSerializer;
//            }

//            // If you chnage enything in the sql query tou need to take care to 
//            // FeedAggregateQuery and DocumentFeedWithFilterQuery as well
//            public async Task<IEnumerable<QuestionFeedDto>> GetAsync(QuestionFeedWithFilterQuery query, CancellationToken token)
//            {
//                const string sqlWithCourse = @"
//SELECT  'q' as type
//,q.CourseId as Course
//,q.Updated as DateTime
//,(select q.Id as Id,
//q.Text as Text,
//q.CourseId as Course,
//(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE(this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
//q.Updated as DateTime,
//q.Language as CultureInfo
//--TODO from cross join
//,x.Id as 'FirstAnswer.User.Id'
//,x.Image as 'FirstAnswer.User.Image'
//,x.Name as 'FirstAnswer.User.Name'
//,x.Text as 'FirstAnswer.Text'
//,x.Created as 'FirstAnswer.DateTime'
//,u.Id as 'User.Id'
//,u.Name as 'User.Name'
//,u.Image as 'User.Image'
//,'Question' as documentType for json path) JsonArray
//,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
//FROM sb.[Question] q
//join sb.[user] u
//	on q.UserId = u.Id
//outer apply (
//select top 1 text, u.id, u.name, u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
//where a.QuestionId = q.Id and state = 'Ok' order by a.created
//) as x
//where
//    q.Updated > GetUtcDATE() - 182
//and q.courseId = @course
//and u.SbCountry = @country
//and q.State = 'Ok'
//order by
//DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE())  +
//DATEDiff(hour, q.Updated, GetUtcDATE()) +
//case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
//OFFSET @page*@pageSize ROWS
//FETCH NEXT @pageSize ROWS ONLY";

//                const string sqlWithoutCourse = @"

//SELECT  'q' as type
//,q.CourseId as Course
//,q.Updated as DateTime
//,(select q.Id as Id,
//q.Text as Text,
//q.CourseId as Course,
//(SELECT count(*) as y0_ FROM sb.[Answer] this_0_ WHERE (this_0_.QuestionId = q.Id and this_0_.State = 'Ok')) as Answers,
//q.Updated as DateTime,
//q.Language as CultureInfo
//--TODO from cross join
//,x.Id as 'FirstAnswer.User.Id'
//,x.Image as 'FirstAnswer.User.Image'
//,x.Name as 'FirstAnswer.User.Name'
//,x.Text as 'FirstAnswer.Text'
//,x.Created as 'FirstAnswer.DateTime'
//,u.Id as 'User.Id'
//,u.Name as 'User.Name'
//,u.Image as 'User.Image'
//,'Question' as documentType for json path) JsonArray
//,case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end as IsFollow
//FROM sb.[Question] q
//join sb.[user] u
//	on q.UserId = u.Id
//outer apply (
//select  top 1 text,u.id,u.name,u.image, a.Created from sb.Answer a join sb.[user] u on a.userid = u.id
//where a.QuestionId = q.Id and state = 'Ok' order by a.created

//) as x
//where 
//    q.Updated > GETUTCDATE() - 182
//and u.SbCountry = @country
//and q.State = 'Ok'
//and (q.CourseId in (select courseId from sb.usersCourses where userid = @userId) or @userid <= 0)
//order by
//DATEDiff(hour, GetUtcDATE() - 180, GetUtcDATE()) +
//DATEDiff(hour, q.Updated, GetUtcDATE()) +
//case when case when (select UserId from sb.UsersRelationship ur where ur.FollowerId = @userId and u.Id = ur.UserId) = u.id then 1 else 0 end = 1 then 0 else DATEDiff(hour, GetUtcDATE() - 7, GetUtcDATE()) end
//OFFSET @page*@pageSize ROWS
//FETCH NEXT @pageSize ROWS ONLY";

//                var sql = query.Course == null ? sqlWithoutCourse : sqlWithCourse;

//                var result = new List<QuestionFeedDto>();
//                using var conn = _dapperRepository.OpenConnection();
//                using var reader = await conn.ExecuteReaderAsync(sql, new
//                {
//                    query.Page,
//                    query.UserId,
//                    Country = query.Country.Id,
//                    query.Course,
//                    query.PageSize

//                });
//                if (reader.Read())
//                {
//                    var colJson = reader.GetOrdinal("JsonArray");
//                    do
//                    {
//                        var v = reader.GetString(colJson);
//                        var questions = _jsonSerializer.Deserialize<IEnumerable<QuestionFeedDto>>(v);
//                        result.Add(questions.First());

//                    } while (reader.Read());
//                }

//                return result;
//            }
//        }
//    }
//}
