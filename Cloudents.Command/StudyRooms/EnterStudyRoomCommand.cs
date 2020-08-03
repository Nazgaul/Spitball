using System;

namespace Cloudents.Command.StudyRooms
{
    public class EnterStudyRoomCommand : ICommand
    {
        public EnterStudyRoomCommand(Guid studyRoomId, long userId, string receipt = null)
        {
            StudyRoomId = studyRoomId;
            UserId = userId;
            Receipt = receipt;
        }

        public Guid StudyRoomId { get; }
        public long UserId { get;  }
        public string? Receipt { get; }

        public string? JwtToken { get; set; }
    }
}