using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using Cloudents.Core.Query;
using NHibernate;
using NHibernate.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Models;

namespace Cloudents.Infrastructure.Database.Query
{
    public class UserCoursesQueryHandler : IQueryHandler<UserWithUniversityQuery, UserProfile>
    {
        private readonly ISession _session;

        public UserCoursesQueryHandler(QuerySession session)
        {
            _session = session.Session;
        }

        //the query cache key is - CacheResultInterceptor.GetCacheKey(_decoratee.GetType(), "GetAsync", new object[] { query });
        //we can disable this if we want to
        //[Cache(TimeConst.Day, "user-courses", true)]
        public async Task<UserProfile> GetAsync(UserWithUniversityQuery query, CancellationToken token)
        {


            //var result = await _session.Query<User>()
            //    .Fetch(f => f.Courses)
            //    .Fetch(f => f.University)
            //    .Fetch(f=>f.Tags)
            //    .Where(w => w.Id == query.Id)
            //    .SingleOrDefaultAsync(token);
            //var university = new UserUniversityQueryProfileDto(result.University.Id, result.University.Extra, result.University.Name);
            //return new UserQueryProfileDto(result.Courses, result.Tags, university);

            var sqlCourses = _session.CreateSQLQuery(@"select CourseId from sb.UsersCourses
where UserId = :id");
            sqlCourses.SetInt64("id", query.Id);

            var coursesFuture = sqlCourses.Future<string>();


            var sqlTags = _session.CreateSQLQuery(@"select tagid from sb.UsersTags
where UserId = :id");
            sqlTags.SetInt64("id", query.Id);

            var tagsFuture = sqlTags.Future<string>();





            IFutureValue<UserUniversityQueryProfileDto> universityFuture = null;

            if (query.UniversityId.HasValue)
            {
                universityFuture = _session.Query<University>()
                    .Where(w => w.Id == query.UniversityId.Value)
                    .Select(s => new UserUniversityQueryProfileDto(s.Id, s.Extra, s.Name)).ToFutureValue();

            }
            

            var courses = await coursesFuture.GetEnumerableAsync(token);
            var tags = await tagsFuture.GetEnumerableAsync(token);
            var university = universityFuture?.Value;

            return new UserProfile
            {
                University = university,
                Courses = courses?.ToList(),
                Tags = tags
            };
        }
    }
}