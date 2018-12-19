using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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
