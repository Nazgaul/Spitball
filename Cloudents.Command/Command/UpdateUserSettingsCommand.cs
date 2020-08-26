namespace Cloudents.Command.Command
{
    public class UpdateUserSettingsCommand : ICommand
    {
        public UpdateUserSettingsCommand(long userId, 
            string firstName, string lastName,
            string? title,
            string? shortParagraph, string? paragraph, string? avatar, string? cover)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Title = title;
            ShortParagraph = shortParagraph;
            Paragraph = paragraph;
            Avatar = avatar;
            Cover = cover;
        }
        public long UserId { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string? Title { get; }

        public string? ShortParagraph { get; }
        public string? Paragraph { get; }

        public string? Avatar { get; }
        public string? Cover { get;  }
    }
}
