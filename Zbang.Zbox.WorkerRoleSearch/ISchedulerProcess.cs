using System;
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public interface ISchedulerProcess
    {
        Task<bool> ExecuteAsync(int index, Func<int, TimeSpan,Task> progressAsync, CancellationToken token);
    }
}
