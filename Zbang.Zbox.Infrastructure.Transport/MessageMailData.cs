using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoBuf.ProtoContract]
    public class MessageMailData : BaseMailData
    {
        protected MessageMailData()
        {

        }
        public MessageMailData(string message,string emailAddress,string senderUserName, string culture)
            :base(emailAddress,culture)
        {
            SenderUserName = senderUserName;
            Message = message;
        }
        [ProtoBuf.ProtoMember(1)]
        public string Message { get; private set; }
        [ProtoBuf.ProtoMember(2)]
        public string SenderUserName { get; private set; }


        public override string MailResover
        {
            get { return MessageResolver; }
        }
    }
}
