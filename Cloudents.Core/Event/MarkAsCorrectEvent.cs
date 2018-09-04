using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
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