using System;

namespace Cloudents.Command.Command
{
    public class EnterTailorEdStudyRoomCodeCommand : ICommand
    {
        public EnterTailorEdStudyRoomCodeCommand(string code, Guid studyRoomId)
        {
            Code = code;
            StudyRoomId = studyRoomId;
        }

        public string Code { get; }
        public Guid StudyRoomId { get;  }


        public long UserId { get; set; }
    }
}