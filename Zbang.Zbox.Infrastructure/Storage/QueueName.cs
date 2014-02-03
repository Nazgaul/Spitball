
namespace Zbang.Zbox.Infrastructure.Storage
{
    public abstract class QueueName
    {
        public abstract string Name
        {
            get;
        }
    }

    public class MailQueueName : QueueName
    {

        public override string Name
        {
            get { return QueueProvider.MailQueueName; }
        }
    }

    public class MailQueueNameNew : QueueName
    {

        public override string Name
        {
            get { return QueueProvider.NewMailQueueName; }
        }
    }

    public class CacheQueueName : QueueName
    {
        public override string Name
        {
            get { return QueueProvider.QueueName; }
        }
    }

    
    public class ThumbnailQueueName : QueueName
    {
        public override string Name
        {
            get { return QueueProvider.ThumbnailQueueName; }
        }
    }

    public class UpdateDomainQueueName : QueueName
    {
        public override string Name
        {
            get { return QueueProvider.UpdateDomainQueueName; }
        }
    }

    public class DownloadQueueName : QueueName
    {
        public override string Name
        {
            get { return QueueProvider.DownloadContentFromUrlPahse2; }
        }

    }



}
