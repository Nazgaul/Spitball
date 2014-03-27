using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class InviteToCloudentsData : BaseMailData
    {
        protected InviteToCloudentsData()
        {

        }
        public InviteToCloudentsData(string senderName, string senderImage, string recepientEmailAddress, string culture, string senderEmail)
            : base(recepientEmailAddress, culture)
        {
            SenderName = senderName;
            SenderImage = senderImage;
            SenderEmail = senderEmail;
        }

        [ProtoMember(1)]
        public string SenderName { get; private set; }
        [ProtoMember(2)]
        public string SenderImage { get; private set; }

        [ProtoMember(3)]
        public string SenderEmail { get; private set; }

        public override string MailResover
        {
            get { return InviteToCloudentsResolver; }
        }
    }
}
