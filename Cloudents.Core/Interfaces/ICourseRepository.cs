using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetCourseByNameAsync(long tutorId, string name, CancellationToken token);
    }
}