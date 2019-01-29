using Cloudents.Core.DTOs.SearchSync;

namespace Cloudents.Core.Message.System
{
    public class QuestionSearchMessage : ISystemQueueMessage
    {
        public bool ShouldInsert { get; private set; }
        public QuestionSearchDto Question { get; private set; }
       

        public QuestionSearchMessage(bool shouldInsert, QuestionSearchDto question)
        {
            ShouldInsert = shouldInsert;
            Question = question;
        }
       
    }
}