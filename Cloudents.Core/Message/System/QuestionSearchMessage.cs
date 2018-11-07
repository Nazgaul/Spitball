using Cloudents.Core.Entities.Search;

namespace Cloudents.Core.Message.System
{
    public class QuestionSearchMessage : ISystemQueueMessage
    {
        public bool ShouldInsert { get; private set; }
        public Question Question { get; private set; }
        //public override dynamic GetData()
        //{
        //    return this;
        //}

        public QuestionSearchMessage(bool shouldInsert,Question question)
        {
            ShouldInsert = shouldInsert;
            Question = question;
        }

        //protected QuestionSearchMessage()
        //{
            
        //}
    }
}