using System;
using Cloudents.Core.Message.System;

namespace Cloudents.Core.Message.Email
{
    public class DocumentPurchasedMessage : ISystemQueueMessage
    {
        public DocumentPurchasedMessage(Guid transactionId)
        {
            TransactionId = transactionId;
        }


        public Guid TransactionId { get; private set; }
    }

    public class AnswerAcceptedMessage : ISystemQueueMessage
    {
        public AnswerAcceptedMessage(Guid transactionId)
        {
            TransactionId = transactionId;
        }


        public Guid TransactionId { get; private set; }
    }


    public class StudentPaymentMessage : ISystemQueueMessage
    {
        public StudentPaymentMessage(Guid studyRoomId)
        {
            StudyRoomId = studyRoomId;
        }

        public Guid StudyRoomId { get; private set; }
    }

    //public class EndTutoringSessionMessage : ISystemQueueMessage
    //{
    //    public EndTutoringSessionMessage(string roomId)
    //    {
    //        RoomId = roomId;
    //    }

    //    public string RoomId { get; private set; }
    //}
}