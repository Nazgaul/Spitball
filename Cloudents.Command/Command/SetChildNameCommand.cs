namespace Cloudents.Command.Command
{
    public class SetChildNameCommand : ICommand
    {
        public SetChildNameCommand(long userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LasttName = lastName;
        }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LasttName { get; set; }
    }
}
