
namespace Zbang.Zbox.Infrastructure.Storage
{

    public abstract class QueueName
    {
        public const string NewMailQueueName = "zboxMailQueue2";
        public const string ThumbnailQueueName = "zboxthumbnailqueue";
        public const string UpdateDomainQueueName = "transactionQueueName";
        //public const string DownloadContentFromUrl = "downloadcontentfromurl";
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

    public class MailQueueNameNew : QueueName
    {

        public override string Name => NewMailQueueName;
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
