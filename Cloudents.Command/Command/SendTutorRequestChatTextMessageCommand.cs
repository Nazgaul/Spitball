namespace Cloudents.Command.Command
{
    public class SendTutorRequestChatTextMessageCommand : ICommand
    {
        public SendTutorRequestChatTextMessageCommand(string course, string text, long userId)
        {
            Course = course;
            Text = text;
            UserId = userId;
        }

        public string Course { get;  }
        public long UserId { get; }
        public string Text { get; }
    }
}