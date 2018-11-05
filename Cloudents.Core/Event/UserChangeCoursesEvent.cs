using Cloudents.Core.Entities.Db;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class UserChangeCoursesEvent : IEvent
    {
        public UserChangeCoursesEvent(User user)
        {
            User = user;
        }

        public User User { get; set; }
    }
}