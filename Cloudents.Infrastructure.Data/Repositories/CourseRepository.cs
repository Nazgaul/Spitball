using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using Cloudents.Core.Entities.Db;
using Cloudents.Core.Enum;
using Cloudents.Core.Interfaces;
using NHibernate.Linq;

namespace Cloudents.Infrastructure.Data.Repositories
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "IOC inject")]
    public class CourseRepository : NHibernateRepository<Course>, ICourseRepository
    {
        public CourseRepository(IIndex<Database, IUnitOfWork> unitOfWorks) : base(unitOfWorks)
        {
        }

        public Task<Course> GetCourseAsync(long universityId, string courseName,CancellationToken token)
        {
            return Session.Query<Course>()
                .Where(w => w.Name == courseName && w.University.Id == universityId)
                .SingleOrDefaultAsync(token);
        }
    }
}