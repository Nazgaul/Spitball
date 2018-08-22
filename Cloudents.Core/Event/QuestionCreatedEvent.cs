using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class QuestionCreatedEvent : IEvent
    {
        public QuestionCreatedEvent(Question question)
        {
            Question = question;
        }

        public Question Question { get; private set; }
    }
}