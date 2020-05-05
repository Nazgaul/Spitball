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
    }


    public class CreateStudyRoomSessionCommandResult : ICommandResult
    {
        public CreateStudyRoomSessionCommandResult(string jwtToken)
        {
            JwtToken = jwtToken;
        }

        public string JwtToken { get; }
    }
}
