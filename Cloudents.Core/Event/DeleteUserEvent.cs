using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class DeleteUserEvent : IEvent
    {
        public DeleteUserEvent(User user)
        {
            User = user;
        }
        public User User { get; }
    }
}