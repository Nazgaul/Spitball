namespace Cloudents.Command.Command.Admin
{
    public class ChangeConversationAssignCommand : ICommand
    {
        public string Identifier { get; }
        public string AssignTo { get; }

        public ChangeConversationAssignCommand(string identifier, string assignTo)
        {
            Identifier = identifier;
            AssignTo = assignTo;
        }
    }
}
