using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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