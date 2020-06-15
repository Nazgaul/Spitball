using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class AddUserBroadcastStudyRoomEvent : IEvent
    {
        public BroadCastStudyRoom BroadCastStudyRoom { get; }
        public User User { get; }

        public AddUserBroadcastStudyRoomEvent(BroadCastStudyRoom broadCastStudyRoom, User user)
        {
            BroadCastStudyRoom = broadCastStudyRoom;
            User = user;
        }
    }
}