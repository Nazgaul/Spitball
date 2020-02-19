using System;

namespace Cloudents.Command.Command
{
    public class StudyRoomSessionParticipantDisconnectedCommand : ICommand
    {
        public StudyRoomSessionParticipantDisconnectedCommand(Guid roomId)
        {
            RoomId = roomId;
        }
        public Guid RoomId { get; }
    }
}
