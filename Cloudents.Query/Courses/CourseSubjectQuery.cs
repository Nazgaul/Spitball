//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Cloudents.Core.DTOs;
//using Cloudents.Core.Entities;
//using NHibernate;
//using NHibernate.Linq;

//namespace Cloudents.Query.Courses
//{
//    public class CourseSubjectQuery : IQuery<SubjectDto>
//    {
//        public CourseSubjectQuery(string courseName)
//        {
//            CourseName = courseName;
//        }

//        private string CourseName { get; }

//        internal sealed class CourseSubjectQueryHandler : IQueryHandler<CourseSubjectQuery, SubjectDto>
//        {
//            private readonly IStatelessSession _session;

//            public CourseSubjectQueryHandler(QuerySession session)
//            {
//                _session = session.StatelessSession;
//            }
//            public async Task<SubjectDto> GetAsync(CourseSubjectQuery query, CancellationToken token)
//            {
//                return await _session.Query<Course>()
//                    .WithOptions(w => w.SetComment(nameof(CourseSubjectQuery)))
//                       .Fetch(f => f.Subject)
//                       .Where(w => w.Id == query.CourseName)
//                       .Select(s => new SubjectDto
//                       {
//                           Name = s.Subject.Name
//                       })
//                       .FirstOrDefaultAsync(token);
//            }
//        }
//    }
//}