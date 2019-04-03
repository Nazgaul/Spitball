using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class ResetUnreadInChatCommand : ICommand
    {
        public ResetUnreadInChatCommand(long userSendingId, IEnumerable<long> toUsersId)
        {
            UserSendingId = userSendingId;
            ToUsersId = toUsersId;
        }

        public long UserSendingId { get; }

        // public string Blob { get; }

        public IEnumerable<long> ToUsersId { get; }
    }
}