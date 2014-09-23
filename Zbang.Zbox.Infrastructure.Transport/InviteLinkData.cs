using ProtoBuf;
using System;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class InviteLinkData
    {
        protected InviteLinkData()
        {

        }
        public InviteLinkData(Guid id, string  boxUrl, DateTime expireTime, long senderId, string recipientEmail)
        {
            Id = id;
            ExpireTime = expireTime;
            SenderId = senderId;
            RecipientEmail = recipientEmail;
            BoxUrl = boxUrl;
        }
        [ProtoMember(1)]
        public Guid Id { get; private set; }
        [ProtoMember(2)]
        public long BoxId { get; private set; }
        [ProtoMember(3)]
        public DateTime ExpireTime { get; private set; }
        [ProtoMember(4)]
        public long SenderId { get; private set; }
        [ProtoMember(5)]
        public string RecipientEmail { get; private set; }

        [ProtoMember(6)]
        public string BoxUrl { get; private set; }
    }
}
