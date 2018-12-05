using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class QuestionDeletedAdminEvent : IEvent
    {

        public QuestionDeletedAdminEvent(Question question)
        {
            Question = question;
        }

        public Question Question { get; }
    }
}