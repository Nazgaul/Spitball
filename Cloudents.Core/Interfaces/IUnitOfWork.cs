using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken token);
        Task RollbackAsync(CancellationToken token);
    }
}