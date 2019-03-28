namespace Cloudents.Command.Command
{
    public class UpdateUserSettingsCommand: ICommand
    {
        public UpdateUserSettingsCommand(long userId, string firstName, string lastName, string description)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Description = description;
        }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        
    }
}
