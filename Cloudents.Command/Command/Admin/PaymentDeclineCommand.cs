using System;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentDeclineCommand : ICommand
    {
        public PaymentDeclineCommand(Guid studyRoomSessionId, long userId)
        {
            StudyRoomSessionId = studyRoomSessionId;
            UserId = userId;
        }

        public Guid StudyRoomSessionId { get; }
        public long UserId { get; }
    }
}