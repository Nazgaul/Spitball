using System;

namespace Cloudents.Command.Command
{
    public class EnrollStudyRoomBroadCastCommand : ICommand
    {
        public EnrollStudyRoomBroadCastCommand(long userId, Guid studyRoomId, string? receipt = null)
        {
            UserId = userId;
            StudyRoomId = studyRoomId;
            Receipt = receipt;
        }

        public long UserId { get;  }

        public Guid StudyRoomId { get; }

        public string? Receipt { get; }

        
    }
}