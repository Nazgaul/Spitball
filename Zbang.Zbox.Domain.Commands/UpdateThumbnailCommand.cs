using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateThumbnailCommand : ICommand
    {
        public UpdateThumbnailCommand(long itemId, string blobName, string fileContent, string md5)
        {
            ItemId = itemId;
            BlobName = blobName;
            FileContent = fileContent;
            Md5 = md5;
        }

        public long ItemId { get; private set; }
        public string BlobName { get;private set; }


        public string FileContent { get; private set; }

        public string Md5 { get; private set; }
    }
}
