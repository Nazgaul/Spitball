using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.StudyRooms
{
    public class EndStudyRoomSessionAfterUserDisconnectedCommand : ICommand
    {
        public EndStudyRoomSessionAfterUserDisconnectedCommand(Guid sessionParticipantDisconnectId)
        {
            SessionParticipantDisconnectId = sessionParticipantDisconnectId;
           
        }
        public Guid SessionParticipantDisconnectId { get; }
    }
}
