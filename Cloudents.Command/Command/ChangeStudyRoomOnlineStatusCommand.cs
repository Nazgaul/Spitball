using System;
using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class ChangeStudyRoomOnlineStatusCommand : ICommand
    {
        public ChangeStudyRoomOnlineStatusCommand(long userId, bool status, Guid studyRoomId)
        {
            UserId = userId;
            Status = status;
            StudyRoomId = studyRoomId;
        }

        public long UserId { get; }
        public bool Status { get; }
        public Guid StudyRoomId { get; }


        public IList<long> OtherUsers { get; set; }
    }
}