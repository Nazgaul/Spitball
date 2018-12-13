using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class QuestionDeletedAdminEvent : IEvent
    {

        public QuestionDeletedAdminEvent(Question question, RegularUser user)
        {
            Question = question;
            User = user;
        }



        public Question Question { get; }

        public RegularUser User { get; }
    }
}