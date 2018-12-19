using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
{
    public class MarkAsCorrectEvent : IEvent
    {
        public MarkAsCorrectEvent(Answer answer)
        {
            Answer = answer;
        }

        public Answer Answer { get; }

    }
}