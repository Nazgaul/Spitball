
namespace Zbang.Zbox.Infrastructure.Storage
{

    public abstract class QueueName
    {
        public const string QueueName2 = "zboxCacheQueue";
        public const string NewMailQueueName = "zboxMailQueue2";
        public const string OrderQueueName = "zboxQueueName";
        public const string UpdateDomainQueueName = "transactionQueueName";
        public const string DownloadContentFromUrl = "downloadcontentfromurl";
        public const string DownloadContentFromUrlPhase2 = "downloadcontentfromurlphase2";
        public abstract string Name
        {
            get;
        }
    }

    public class OrderQueueName : QueueName
    {

        public override string Name
        {
            get { return OrderQueueName; }
        }
    }

    public class MailQueueNameNew : QueueName
    {

        public override string Name
        {
            get { return NewMailQueueName; }
        }
    }

    public class CacheQueueName : QueueName
    {
        public override string Name
        {
            get { return QueueName2; }
        }
    }

    public class UpdateDomainQueueName : QueueName
    {
        public override string Name
        {
            get { return UpdateDomainQueueName; }
        }
    }

    public class DownloadQueueName : QueueName
    {
        public override string Name
        {
            get { return DownloadContentFromUrlPhase2; }
        }

    }



}
