using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IReadRepository
    {
        Task<T> WithConnectionAsync<T>(Func<IDbConnection, Task<T>> getData, CancellationToken token);
    }
}