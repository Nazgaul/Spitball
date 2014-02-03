using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class DeleteItemMail : MailQueueData
    {
        public const string resolverName = "DeleteItem";
        public DeleteItemMail(string boxName, string boxItemName, long boxId, string userNameThatDeleted, long userThatDeleteId)
            : base(resolverName)
        {
            BoxName = boxName;
            BoxItemName = boxItemName;
            BoxId = boxId;
            UserNameThatDelete = userNameThatDeleted;
            UserThatDeleteId = userThatDeleteId;
        }

        [DataMember]
        public string BoxName { get; private set; }

        [DataMember]
        public string BoxItemName { get; private set; }

        [DataMember]
        public long BoxId { get; private set; }

        [DataMember]
        public string UserNameThatDelete { get; private set; }

        [DataMember]
        public long UserThatDeleteId { get; private set; }
    }
}
