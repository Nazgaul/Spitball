namespace Cloudents.Command.Command
{
    public class UpdateUserSettingsCommand : ICommand
    {
        public UpdateUserSettingsCommand(long userId, 
            string firstName, string lastName,
            string? title,
            string? shortParagraph, string? paragraph)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Title = title;
            ShortParagraph = shortParagraph;
            Paragraph = paragraph;
        }
        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string? Title { get; }

        public string? ShortParagraph { get; }
        public string? Paragraph { get; }
    }
}
