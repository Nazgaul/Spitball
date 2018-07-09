using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Linq;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    public class CourseRepository : NHibernateRepository<Course> ,ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public Task<Course> GetCourseAsync(long universityId, string courseName, CancellationToken token)
        {
            return Session.Query<Course>()
                .Where(w => w.Name == courseName && w.University.Id == universityId)
                .SingleOrDefaultAsync(token);
        }
    }
}