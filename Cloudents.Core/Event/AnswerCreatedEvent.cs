using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
{
    public class AnswerCreatedEvent : IEvent
    {
        public AnswerCreatedEvent(Answer answer)
        {
            Answer = answer;
        }

        public Answer Answer { get; private set; }
    }
}