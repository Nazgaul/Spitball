namespace Cloudents.Command.Command
{
    public class UpdateUserImageCommand : ICommand
    {
        public UpdateUserImageCommand(long userId, string imagePath)
        {
            UserId = userId;
            ImagePath = imagePath;
        }

        public long UserId { get; private set; }
        public string ImagePath { get; private set; }
    }
}