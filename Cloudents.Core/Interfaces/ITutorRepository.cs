using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface ITutorRepository : IRepository<Tutor>
    {
        Task<Tutor> GetTailorEdTutorAsync(CancellationToken token);
    }
}