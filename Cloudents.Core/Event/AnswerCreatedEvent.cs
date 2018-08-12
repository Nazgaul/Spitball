namespace Cloudents.Core.Event
{
    public class AnswerCreatedEvent
    {
        public long QuestionUserId { get; set; }
        public long QuestionId { get; set; }
    }
}