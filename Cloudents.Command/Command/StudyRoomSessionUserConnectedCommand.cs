using System;

namespace Cloudents.Command.Command
{
    public class StudyRoomSessionUserConnectedCommand : ICommand
    {
        public Guid RoomId { get; }
        public string SessionId { get; }
        public long UserId { get; }

        public StudyRoomSessionUserConnectedCommand(Guid roomId,string sessionId, long userId)
        {
            RoomId = roomId;
            SessionId = sessionId;
            UserId = userId;
        }
        
    }
}
