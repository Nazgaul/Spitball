
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch
{
    public interface IJob
    {
        Task Run(CancellationToken cancellationToken);
    }
}
