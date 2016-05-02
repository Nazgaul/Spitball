
namespace Zbang.Zbox.Infrastructure.Storage
{

    public abstract class QueueName
    {
       // public const string QueueName2 = "zboxCacheQueue";
        public const string NewMailQueueName = "zboxMailQueue2";
        public const string UpdateDomainQueueName = "transactionQueueName";
        public const string DownloadContentFromUrl = "downloadcontentfromurl";
        public const string DownloadContentFromUrlPhase2 = "downloadcontentfromurlphase2";
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

    //public class CacheQueueName : QueueName
    //{
    //    public override string Name => QueueName2;
    //}

    public class UpdateDomainQueueName : QueueName
    {
        public override string Name => UpdateDomainQueueName;
    }

    public class DownloadQueueName : QueueName
    {
        public override string Name => DownloadContentFromUrlPhase2;
    }



}
