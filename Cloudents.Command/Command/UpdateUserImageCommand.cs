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

        public long UserId { get; }
        public string ImagePath { get; }
        public string FileName { get; }
    }
}