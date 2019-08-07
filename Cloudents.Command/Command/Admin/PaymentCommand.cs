using System;

namespace Cloudents.Command.Command.Admin
{
    public class PaymentCommand: ICommand
    {
        public PaymentCommand(string userKey, string tutorKey, decimal amount, Guid studyRoomSessionId)
        {
            UserKey = userKey;
            TutorKey = tutorKey;
            Amount = amount;
            StudyRoomSessionId = studyRoomSessionId;
        }
        public string UserKey { get;  }
        public string TutorKey { get;}
        public decimal Amount { get;  }
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
