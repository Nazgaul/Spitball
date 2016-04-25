using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateThumbnailCommand : ICommand
    {
        public UpdateThumbnailCommand(long itemId, string blobName, string fileContent)
        {
            ItemId = itemId;
            BlobName = blobName;
            FileContent = fileContent;
        }

        public long ItemId { get; private set; }
        public string BlobName { get;private set; }


        public string FileContent { get; set; }
    }
}
