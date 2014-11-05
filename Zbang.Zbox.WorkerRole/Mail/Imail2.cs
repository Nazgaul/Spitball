using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal interface IMail2
    {
        bool Execute(BaseMailData data);
    }
}
