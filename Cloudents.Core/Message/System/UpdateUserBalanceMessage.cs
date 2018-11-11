using System.Collections.Generic;

namespace Cloudents.Core.Message.System
{
    public class UpdateUserBalanceMessage: ISystemQueueMessage
    {
        public UpdateUserBalanceMessage(IEnumerable<long> userIds)
        {
            UserIds = userIds;
        }

        protected UpdateUserBalanceMessage()
        {
            
        }
        public IEnumerable<long> UserIds { get; private set; }
    }


    public class AddUserTagMessage : ISystemQueueMessage
    {
        public AddUserTagMessage(long userId, string tag)
        {
            UserId = userId;
            Tag = tag;
        }

        
        public long UserId { get; private set; }
        public string Tag { get; private set; }
    }
}