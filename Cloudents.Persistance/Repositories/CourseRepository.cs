using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(ISession session) : base(session)
        {
        }

        public async Task<IEnumerable<Course>> GetCoursesBySubjectIdAsync(long subjectId, CancellationToken token)
        {
            return await Session.Query<Course>()
                .Fetch(f => f.Subject)
                .Where(w => w.Subject.Id == subjectId)
                .ToListAsync(token);
        }
    }
}
