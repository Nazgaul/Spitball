using System;

namespace Cloudents.Command.StudyRooms
{
    public class CreateStudyRoomSessionCommand : ICommand
    {
        public CreateStudyRoomSessionCommand(Guid studyRoomId,
            long userId)
        {
            StudyRoomId = studyRoomId;
            UserId = userId;
        }
        public Guid StudyRoomId { get; }


        public long UserId { get; }

        public string JwtToken { get; set; }
    }
}
