using System;

namespace Cloudents.Command.Command
{
    public class StudyRoomSessionParticipantReconnectedCommand : ICommand
    {
        public StudyRoomSessionParticipantReconnectedCommand(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }
    }
}
