using System;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand: ICommand
    {
        public PaymentCommand(long userId, long tutorId, decimal studentPay,
            Guid studyRoomSessionId)
        {
            UserId = userId;
            TutorId = tutorId;
            StudentPay = studentPay;
            StudyRoomSessionId = studyRoomSessionId;
        }

        public long UserId { get;  }
        public long TutorId { get;}
        public decimal StudentPay { get;  }
        public Guid StudyRoomSessionId { get;  }
    }

    public class DeclinePaymentCommand : ICommand
    {
        public DeclinePaymentCommand(Guid studyRoomSessionId)
        {
            StudyRoomSessionId = studyRoomSessionId;
        }

        public Guid StudyRoomSessionId { get; }
    }
}
