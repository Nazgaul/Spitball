using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class CourseRepository : NHibernateRepository<Course> ,ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public Task<Course> GetCourseAsync(long universityId, string courseName, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}