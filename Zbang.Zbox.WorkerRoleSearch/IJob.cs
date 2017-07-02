
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public interface IJob
    {
        Task RunAsync(CancellationToken cancellationToken);
        string Name { get; }

    }
}
