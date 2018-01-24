using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloudents.Core.Interfaces
{
    public interface IUpdateAffiliate
    {
        Task ExecuteAsync(int index, Func<int, Task> progressAsync, CancellationToken token);
    }
}