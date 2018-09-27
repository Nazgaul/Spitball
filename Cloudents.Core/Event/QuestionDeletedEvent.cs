using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class QuestionDeletedEvent : IEvent
    {
        public QuestionDeletedEvent(Question question)
        {
            Question = question;
        }

        public Question Question { get; }
    }
}