using Cloudents.Core.Enum;

namespace Cloudents.Command.Command.Admin
{
    public class ChangeConversationAssignCommand : ICommand
    {
        public string Identifier { get; }
        public ChatRoomAssign AssignTo { get; }

        public ChangeConversationAssignCommand(string identifier, ChatRoomAssign assignTo)
        {
            Identifier = identifier;
            AssignTo = assignTo;
        }
    }
}
