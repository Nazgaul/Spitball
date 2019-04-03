using System;
using System.Collections.Generic;
using System.Text;

namespace Cloudents.Command.Command
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId)
        {
            StudyRoomId = studyRoomId;
        }
        public Guid StudyRoomId { get; set; }
    }
}
