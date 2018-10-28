namespace Cloudents.Core.Storage
{
    public sealed class QueueName
    {
        public const string QuestionsQueueName = "questions";
        public const string EmailQueueName = "emails";
        public const string SmsQueueName = "sms";

        public string Name { get; }

        private QueueName(string name)
        {
            Name = name;
        }
        public static readonly QueueName QuestionQueue = new QueueName(QuestionsQueueName);
        public static readonly QueueName EmailQueue = new QueueName(EmailQueueName);
        public static readonly QueueName SmsQueue = new QueueName(SmsQueueName);


    }
}