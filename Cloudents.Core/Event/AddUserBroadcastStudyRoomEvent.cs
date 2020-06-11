using Cloudents.Core.Entities;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Event
{
    public class AddUserBroadcastStudyRoomEvent : IEvent
    {
        public BroadCastStudyRoom BroadCastStudyRoom { get; private set; }

        public AddUserBroadcastStudyRoomEvent(BroadCastStudyRoom broadCastStudyRoom)
        {
            BroadCastStudyRoom = broadCastStudyRoom;
        }
    }
}