namespace Cloudents.Core.Storage
{
    public sealed class QueueName
    {
        public const string EmailQueueName = "email";
        //public const string EmailQueueName2 = "email2";
        //public const string SmsQueueName = "sms";
        public const string BackgroundQueueName = "background";

        public string Name { get; }

        private QueueName(string name)
        {
            Name = name;
        }
        public static readonly QueueName EmailQueue = new QueueName(EmailQueueName);
        //public static readonly QueueName EmailQueue2 = new QueueName(EmailQueueName2);
        public static readonly QueueName BackgroundQueue = new QueueName(BackgroundQueueName);
        //public static readonly QueueName SmsQueue = new QueueName(SmsQueueName);


    }
}