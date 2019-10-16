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

    //public class AnswerAcceptedMessage : ISystemQueueMessage
    //{
    //    public AnswerAcceptedMessage(long questionId)
    //    {
    //        QuestionId = questionId;
    //    }


    //    public long QuestionId { get; private set; }
    //}


   

}