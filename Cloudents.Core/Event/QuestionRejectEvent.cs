using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class QuestionRejectEvent : IEvent
    {
        public QuestionRejectEvent(User user)
        {
            User = user;
        }

        public User User { get; }
    }
}