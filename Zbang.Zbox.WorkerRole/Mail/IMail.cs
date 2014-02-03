using Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer;

namespace Zbang.Zbox.WorkerRole.Mail
{
    internal interface IMail
    {
        bool Excecute(MailQueueData data);
    }
}
