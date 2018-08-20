using System;

namespace Cloudents.Core.Event
{
    public class AnswerCreatedEvent
    {
        public AnswerCreatedEvent(long questionId, Guid answerId)
        {
            QuestionId = questionId;
            AnswerId = answerId;
        }

        // public long QuestionUserId { get; set; }
        public long QuestionId { get; set; }

        public Guid AnswerId { get; set; }
    }
}