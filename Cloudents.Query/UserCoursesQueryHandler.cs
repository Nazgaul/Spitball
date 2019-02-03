using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;
using Cloudents.Core.Models;
using Cloudents.Query.Query;
using NHibernate;
using NHibernate.Linq;

namespace Cloudents.Query
{
    public class UserCoursesQueryHandler : IQueryHandler<UserWithUniversityQuery, UserProfile>
    {
        private readonly IStatelessSession _session;

        public UserCoursesQueryHandler(QuerySession session)
        {
            _session = session.StatelessSession;
        }

        //the query cache key is - CacheResultInterceptor.GetCacheKey(_decoratee.GetType(), "GetAsync", new object[] { query });
        //we can disable this if we want to
        //[Cache(TimeConst.Day, "user-courses", true)]
        public async Task<UserProfile> GetAsync(UserWithUniversityQuery query, CancellationToken token)
        {

            var sqlCourses = _session.CreateSQLQuery(@"select CourseId from sb.UsersCourses
where UserId = :id");
            sqlCourses.SetInt64("id", query.Id);

            var coursesFuture = sqlCourses.Future<string>();


            var sqlTags = _session.CreateSQLQuery(@"select tagid from sb.UsersTags
where UserId = :id");
            sqlTags.SetInt64("id", query.Id);

            var tagsFuture = sqlTags.Future<string>();





            IFutureValue<UserUniversityQueryProfileDto> universityFuture = null;

           // if (query.UniversityId.HasValue)
            //{
               universityFuture = _session.Query<RegularUser>()
                   .Fetch(f=>f.University)
                    .Where(w => w.Id == query.Id && w.University != null)
                   
                    .Select(s => new UserUniversityQueryProfileDto(s.University.Id, s.University.Extra,
                       s.University.Name,s.University.Country)).ToFutureValue();

           // }
            

            var courses = await coursesFuture.GetEnumerableAsync(token);
            var tags = await tagsFuture.GetEnumerableAsync(token);
            var university = universityFuture?.Value;

            return new UserProfile
            {
                University = university,
                Courses = courses?.ToList(),
                Tags = tags,
                
            };
        }
    }
}