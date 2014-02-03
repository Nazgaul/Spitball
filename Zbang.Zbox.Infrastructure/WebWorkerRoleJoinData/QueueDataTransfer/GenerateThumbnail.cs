using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class GenerateThumbnail : MailQueueData
    {
        public const string resolverName = "GenerateThumbnail";
        public GenerateThumbnail(string blobName, long itemId)
            : base(resolverName)
        {
            BlobName = blobName;
            ItemId = itemId;
        }

        [DataMember]
        public string BlobName { get; private set; }
        [DataMember]
        public long ItemId { get; private set; }
    }
}
