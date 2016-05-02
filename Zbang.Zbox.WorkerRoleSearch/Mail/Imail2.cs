using System.Threading;
using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRoleSearch.Mail
{
    internal interface IMail2
    {
        Task<bool> ExecuteAsync(BaseMailData data, CancellationToken token);
    }
}
