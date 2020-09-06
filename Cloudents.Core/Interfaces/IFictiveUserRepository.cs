using System.Threading;
using System.Threading.Tasks;
using Cloudents.Core.Entities;

namespace Cloudents.Core.Interfaces
{
    public interface IFictiveUserRepository : IRepository<SystemUser>
    {
        Task<SystemUser> GetRandomFictiveUserAsync(int index, CancellationToken token);
    }
}