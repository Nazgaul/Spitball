using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class SubscribeToBoxMail : MailQueueData
    {
        public const string resolverName = "SubscribeToBox";
        public SubscribeToBoxMail(string boxName, long boxId, string userName, long userId)
            : base(resolverName)
        {
            BoxName = boxName;
            BoxId = boxId;
            UserName = userName;
            UserId = userId;
        }

        [DataMember]
        public string BoxName { get; private set; }

        [DataMember]
        public long BoxId { get; private set; }

        [DataMember]
        public string UserName { get; private set; }

        [DataMember]
        public long UserId { get; private set; }
    }
}
