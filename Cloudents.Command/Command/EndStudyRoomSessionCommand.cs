using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class EndStudyRoomSessionCommand : ICommand
    {
        public EndStudyRoomSessionCommand(Guid studyRoomId, Guid studyRoomSessionId)
        {
            StudyRoomId = studyRoomId;
            StudyRoomSessionId = studyRoomSessionId;
        }
        public Guid StudyRoomId { get; set; }
        public Guid StudyRoomSessionId { get; set; }
    }
}
