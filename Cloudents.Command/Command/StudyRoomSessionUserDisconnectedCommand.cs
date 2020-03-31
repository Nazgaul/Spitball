using System;

namespace Cloudents.Command.Command
{
    public class StudyRoomSessionUserDisconnectedCommand : ICommand
    {
        public Guid RoomId { get;  }
        public string SessionId { get;  }
        public long UserId { get;  }
        public TimeSpan TimeInRoom { get;  }

        public StudyRoomSessionUserDisconnectedCommand(Guid roomId, string sessionId,long userId, TimeSpan timeInRoom)
        {
            RoomId = roomId;
            SessionId = sessionId;
            UserId = userId;
            TimeInRoom = timeInRoom;
        }
    }
}
