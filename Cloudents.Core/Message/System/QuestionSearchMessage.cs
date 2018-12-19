using Cloudents.Application.DTOs.SearchSync;

namespace Cloudents.Application.Message.System
{
    public class QuestionSearchMessage : ISystemQueueMessage
    {
        public bool ShouldInsert { get; private set; }
        public QuestionSearchDto Question { get; private set; }
        //public override dynamic GetData()
        //{
        //    return this;
        //}

        public QuestionSearchMessage(bool shouldInsert, QuestionSearchDto question)
        {
            ShouldInsert = shouldInsert;
            Question = question;
        }
       
    }
}