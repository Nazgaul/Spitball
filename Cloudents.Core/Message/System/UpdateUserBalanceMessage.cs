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
}