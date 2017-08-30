﻿using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class InviteMailData : BaseMailData
    {
        protected InviteMailData()
        {
        }

        public InviteMailData(string inviterName, string boxName, string boxUrl
            , string emailAddress, string culture, string inviterImage, string inviterEmail, long? receiverId, long boxId)
            : base(emailAddress, culture)
        {
            BoxId = boxId;
            ReceiverId = receiverId;
            InviterName = inviterName;
            BoxName = boxName;
            BoxUrl = boxUrl;
            InviterImage = inviterImage;
            InviterEmail = inviterEmail;
        }
        [ProtoMember(1)]
        public string InviterName { get; private set; }
        [ProtoMember(2)]
        public string BoxName { get; private set; }
        [ProtoMember(3)]
        public string BoxUrl { get; private set; }
        [ProtoMember(4)]
        public string InviterImage { get; private set; }

        [ProtoMember(5)]
        public string InviterEmail { get; private set; }

        [ProtoMember(6)]
        public long? ReceiverId { get; private set; }

        [ProtoMember(7)]
        public long BoxId { get; private set; }

        public override string MailResolver => InviteResolver;
    }
}
