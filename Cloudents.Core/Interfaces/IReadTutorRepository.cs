using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IReadTutorRepository : IRepository<ReadTutor>
    {
        Task<ReadTutor?> GetReadTutorAsync(long userId, CancellationToken token);
        Task AddOrUpdateAsync(ReadTutor entity, CancellationToken token);
    }
}