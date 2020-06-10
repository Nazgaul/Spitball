namespace Cloudents.Command.Command
{
    public class UpdateUserSettingsCommand : ICommand
    {
        public UpdateUserSettingsCommand(long userId, 
            string firstName, string lastName,
            string description,
            string bio)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Description = description;
            Bio = bio;
        }
        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string Description { get; }

        public string Bio { get; }
    }
}
