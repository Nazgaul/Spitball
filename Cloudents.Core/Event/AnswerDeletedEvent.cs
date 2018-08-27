using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
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