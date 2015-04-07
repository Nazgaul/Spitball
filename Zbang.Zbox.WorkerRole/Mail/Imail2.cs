using System.Threading.Tasks;
using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal interface IMail2
    {
        Task<bool> ExecuteAsync(BaseMailData data);
    }
}
