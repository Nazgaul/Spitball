using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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
