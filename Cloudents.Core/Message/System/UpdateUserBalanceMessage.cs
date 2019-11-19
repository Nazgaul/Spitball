using System.Collections.Generic;

namespace Cloudents.Core.Message.System
{
    public class UpdateUserBalanceMessage : ISystemQueueMessage
    {
        public UpdateUserBalanceMessage(IEnumerable<long> usersIds)
        {
            UsersIds = usersIds;
        }
        public IEnumerable<long> UsersIds { get; set; }
    }
}