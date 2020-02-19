namespace Cloudents.Command.Command
{
    public class UpdateUserImageCommand : ICommand
    {
        public UpdateUserImageCommand(long userId, string imagePath, string fileName)
        {
            UserId = userId;
            ImagePath = imagePath;
            FileName = fileName;
        }

        public long UserId { get; private set; }
        public string ImagePath { get; private set; }
        public string FileName { get; private set; }
    }
}