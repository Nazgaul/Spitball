namespace Cloudents.Core.Storage
{
    public sealed class QueueName
    {
        public const string QuestionsQueueName = "questions";
        public string Name { get; }

        private  QueueName(string name)
        {
            Name = name;
        }
        public static readonly QueueName QuestionQueue = new QueueName(QuestionsQueueName);
    }
}