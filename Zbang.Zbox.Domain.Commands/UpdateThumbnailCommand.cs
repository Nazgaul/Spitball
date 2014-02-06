using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateThumbnailCommand : ICommand
    {
        public UpdateThumbnailCommand(long itemId, string thumbnailUrl, string blobName, string oldBlobName, string fileContent)
        {
            ItemId = itemId;
            ThumbnailUrl = thumbnailUrl;
            BlobName = blobName;
            OldBlobName = oldBlobName;
            FileContent = fileContent;
        }

        public long ItemId { get; private set; }
        public string BlobName { get;private set; }

        public string OldBlobName { get; private set; }
        public string ThumbnailUrl { get; private set; }

        public string FileContent { get; set; }
    }
}
