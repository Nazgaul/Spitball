using System.Collections.Generic;

namespace Cloudents.Command.Command
{
    public class ResetUnreadInChatCommand : ICommand
    {
        public ResetUnreadInChatCommand(long userSendingId, string identifier)
        {
            UserSendingId = userSendingId;
            Identifier = identifier;
        }

        public long UserSendingId { get; }

        // public string Blob { get; }

        public string Identifier { get; }
    }
}