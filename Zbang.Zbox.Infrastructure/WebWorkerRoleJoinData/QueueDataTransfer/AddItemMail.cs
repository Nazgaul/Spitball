using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class AddItemMail : MailQueueData
    {
        public const string resolverName = "AddItem";
        public AddItemMail(long boxid, long uploaderId, long itemid)
            : base(resolverName)
        {

            BoxId = boxid;
            UploaderId = uploaderId;
            ItemId = itemid;
        }
        [DataMember]
        public long BoxId { get; private set; }

        [DataMember]
        public long UploaderId { get; private set; }

        [DataMember]
        public long ItemId { get; set; }
    }
}
