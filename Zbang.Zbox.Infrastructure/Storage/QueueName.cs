
namespace Zbang.Zbox.Infrastructure.Storage
{
    public abstract class QueueName
    {
        public const string MailQueueName = "zboxMailQueue2";
        public const string ThumbnailQueueName = "zboxthumbnailqueue";
        public const string UpdateDomainQueueName = "transactionQueueName";
        public const string Scheduler = "zbox-scheduler";

        public abstract string Name
        {
            get;
        }
    }

    public class SchedulerQueueName : QueueName
    {
        public override string Name => Scheduler;
    }

    public class MailQueueName : QueueName
    {
        public override string Name => MailQueueName;
    }

    public class UpdateDomainQueueName : QueueName
    {
        public override string Name => UpdateDomainQueueName;
    }

    public class ThumbnailQueueName : QueueName
    {
        public override string Name => ThumbnailQueueName;
    }
}
