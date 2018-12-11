using Cloudents.Core.Entities.Db;
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
