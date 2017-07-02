
using System.Threading;
using System.Threading.Tasks;

namespace Zbang.Zbox.WorkerRoleSearch.DomainProcess
{
    internal interface IDomainProcess
    {
        Task<bool> ExecuteAsync(Infrastructure.Transport.DomainProcess data, CancellationToken token);
    }
}
