using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync(CancellationToken token);
        Task CommitAsync() => CommitAsync(CancellationToken.None);
        Task RollbackAsync(CancellationToken token);


    }
}