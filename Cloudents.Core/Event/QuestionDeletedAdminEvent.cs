using Cloudents.Application.Interfaces;
using Cloudents.Domain.Entities;

namespace Cloudents.Application.Event
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