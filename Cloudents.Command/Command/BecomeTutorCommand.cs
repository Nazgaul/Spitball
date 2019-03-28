namespace Cloudents.Command.Command
{
    public class BecomeTutorCommand : ICommand
    {
        public BecomeTutorCommand(long userId, string bio)
        {
            UserId = userId;
            Bio = bio;
        }

        public long UserId { get; }


        public string Bio { get; }
    }
}