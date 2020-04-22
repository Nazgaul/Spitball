using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;
using NHibernate;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Persistence.Repositories
{
    public class CourseSubjectRepository : NHibernateRepository<CourseSubject>, ICourseSubjectRepository
    {
        public CourseSubjectRepository(ISession session) : base(session)
        {
        }
        public async Task<CourseSubject?> GetCourseSubjectByName(string name, CancellationToken token)
        {
            return await Session.QueryOver<CourseSubject>()
                .Where(w => w.Name == name).SingleOrDefaultAsync<CourseSubject>(token);
        }
    }
}
