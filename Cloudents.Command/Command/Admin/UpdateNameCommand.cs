namespace Cloudents.Command.Command.Admin
{
    public class UpdateNameCommand : ICommand
    {
        public UpdateNameCommand(long userId, string firstName, string lastName)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
        }
        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
    }
}
