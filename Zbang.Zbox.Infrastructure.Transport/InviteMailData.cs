using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class InviteMailData : BaseMailData
    {
        protected InviteMailData()
        {

        }
        public InviteMailData(string invitorName, string boxName, string boxUrl
            , string emailAddress, string culture, string invitorImage, string invitorEmail)
            : base(emailAddress, culture)
        {
            InvitorName = invitorName;
            BoxName = boxName;
            BoxUrl = boxUrl;
            InvitoryImage = invitorImage;
            InvitoryEmail = invitorEmail;
        }
        [ProtoMember(1)]
        public string InvitorName { get; private set; }
        [ProtoMember(2)]
        public string BoxName { get; private set; }
        [ProtoMember(3)]
        public string BoxUrl { get; private set; }
        [ProtoMember(4)]
        public string InvitoryImage { get; private set; }

        [ProtoMember(5)]
        public string InvitoryEmail { get; private set; }

        public override string MailResover
        {
            get { return BaseMailData.InviteResolver; }
        }
    }
}
