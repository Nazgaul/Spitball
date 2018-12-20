using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class AnswerDeletedAdminEvent : IEvent
    {
        public AnswerDeletedAdminEvent(Answer answer)
        {
            Answer = answer;
        }
        public Answer Answer { get; set; }
    }
}
