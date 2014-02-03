using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class InviteMail : MailQueueData
    {
        public const string resolverName = "Invite";
        public InviteMail(long boxid, List<string> to, string personalNote,
            string senderUserName)
            : base(resolverName)
        {
            BoxId = boxid;
            To = to;
            PersonalNote = personalNote;
            SenderUserName = senderUserName;
        }



        [DataMember]
        public long BoxId { get; private set; }
        [DataMember]
        public List<string> To { get; private set; }
        [DataMember]
        public string PersonalNote { get; private set; }
        [DataMember]
        public string SenderUserName { get; private set; }
        
    }
}
