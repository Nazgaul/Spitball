using System.Collections.Generic;
using Cloudents.Core.Entities.Search;
using NotImplementedException = System.NotImplementedException;

namespace Cloudents.Core.Message.System
{
    public class QuestionSearchMessage : BaseSystemMessage
    {
        public bool ShouldInsert { get; private set; }
        public Question Question { get; private set; }
        public override SystemMessageType Type => SystemMessageType.QuestionSearch;
        public override dynamic GetData()
        {
            return this;
        }

        public QuestionSearchMessage(bool shouldInsert,Question question)
        {
            ShouldInsert = shouldInsert;
            Question = question;
        }

        protected QuestionSearchMessage()
        {
            
        }
    }

    public class UpdateUserBalanceMessage: BaseSystemMessage
    {
        public UpdateUserBalanceMessage(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        public override SystemMessageType Type => SystemMessageType.UpdateBalance;
        public override dynamic GetData()
        {
            return UserIds;
        }

        protected UpdateUserBalanceMessage()
        {
            
        }
        public IEnumerable<long> UserIds { get; private set; }
    }
}