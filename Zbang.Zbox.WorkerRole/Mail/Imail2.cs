using Zbang.Zbox.Infrastructure.Transport;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal interface Imail2
    {
        bool Excecute(BaseMailData data);
    }
}
