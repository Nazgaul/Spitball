using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
{
    public class AnswerDeletedEvent : IEvent
    {
        public AnswerDeletedEvent(Answer answer)
        {
            Answer = answer;
        }

        public Answer Answer { get; private set; }
    }
}