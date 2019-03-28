namespace Cloudents.Command.Command
{
    public class EditTutorProfileCommand: ICommand
    {
        public EditTutorProfileCommand(long userId, string name, string lastName, string bio, string description)
        {
            UserId = userId;
            Name = name;
            LastName = lastName;
            Bio = bio;
            Description = description;
        }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string Description { get; set; }
    }
}
