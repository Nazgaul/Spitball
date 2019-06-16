using Cloudents.Core.Enum;

namespace Cloudents.Command.Command.Admin
{
    public class ChangeConversationStatusCommand : ICommand
    {
        public string Identifier { get; }
        public ChatRoomStatus Status { get; }

        public ChangeConversationStatusCommand(string identifier, ChatRoomStatus modelStatus)
        {
            Identifier = identifier;
            Status = modelStatus;
        }
    }
}