using Zbang.Zbox.Infrastructure.Commands;

namespace Zbang.Zbox.Domain.Commands
{
    public class UpdateFileCommand : ICommand
    {
        public UpdateFileCommand(long userId, long itemId, string blobName)
        {
            UserId = userId;
            ItemId = itemId;
            BlobName = blobName;
        }
        public long UserId { get; private set; }
        public long ItemId { get; private set; }

        public string BlobName { get;private set; }
    }
}
