//using Cloudents.Core.Entities;
//using Cloudents.Core.Models;
//using NHibernate;
//using NHibernate.Linq;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Cloudents.Query.Query
//{
//    public class UserDataQuery : IQuery<UserProfile>
//    {
//        public UserDataQuery(long id)
//        {
//            Id = id;
//        }

//        private long Id { get; }


//        internal sealed class UserProfileQueryHandler : IQueryHandler<UserDataQuery, UserProfile>
//        {
//            private readonly IStatelessSession _session;

//            public UserProfileQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }

//            //the query cache key is - CacheResultInterceptor.GetCacheKey(_decoratee.GetType(), "GetAsync", new object[] { query });
//            //we can disable this if we want to
//            //[Cache(TimeConst.Day, "user-courses", true)]
//            public async Task<UserProfile> GetAsync(UserDataQuery query, CancellationToken token)
//            {
//                //TODO: add cache in here

//                var sqlCourses = _session.CreateSQLQuery(@"select CourseId from sb.UsersCourses
//where UserId = :id");
//                sqlCourses.SetInt64("id", query.Id);

//                var coursesFuture = sqlCourses.Future<string>();


////                var sqlTags = _session.CreateSQLQuery(@"Select  distinct TagId from sb.DocumentsTags where DocumentId in (
////Select id from sb.Document where CourseName in (
////Select CourseId from sb.UsersCourses where userid = :id
////)
////)");
////                sqlTags.SetInt64("id", query.Id);

//                //var tagsFuture = sqlTags.Future<string>();

//                //var universityFuture = _session.Query<RegularUser>()
//                //    .Fetch(f => f.University)
//                //    .Where(w => w.Id == query.Id && w.University != null)

//                //    .Select(s => new UserUniversityQueryProfileDto(s.University.Id, s.University.Extra,
//                //        s.University.Name, s.University.Country)).ToFutureValue();


//                var courses = await coursesFuture.GetEnumerableAsync(token);
//               // var tags = await tagsFuture.GetEnumerableAsync(token);
//                var university = universityFuture?.Value;

//                return new UserProfile
//                {
//                    //University = university,
//                    Courses = courses?.ToList(),
//                };
//            }
//        }
//    }



//}