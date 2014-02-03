using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zbang.Zbox.Infrastructure.WebWorkerRoleJoinData.QueueDataTransfer
{
    [DataContract]
    public class MessageMail : MailQueueData
    {
        public const string resolverName = "Message";

        public MessageMail(string senderName, string senderEmail, List<string> to,
            string personalNote, string url)
            : base(resolverName)
        {
            SenderName = senderName;
            SenderEmail = senderEmail;
            To = to;
            PersonalNote = personalNote;
            //BoxUid = boxuid;
            //ItemUid = itemuid;
            Url = url;
        }

        [DataMember]
        public List<string> To { get; private set; }
        [DataMember]
        public string PersonalNote { get; private set; }
        [DataMember]
        public string SenderName { get; private set; }
        [DataMember]
        public string SenderEmail { get; set; }
        /*didnt remove this yet because dont want to break the scheme*/
        [DataMember]
        public string BoxUid { get; set; }
        [DataMember]
        public string ItemUid { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
