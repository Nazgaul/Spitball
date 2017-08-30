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

        public long ItemId { get; }
        public string BlobName { get; }


        public string FileContent { get; }

        public string Md5 { get; }
    }
}
