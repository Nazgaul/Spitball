using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class InviteLinkData
    {
        protected InviteLinkData()
        {

        }
        public InviteLinkData(Guid id, long boxId, DateTime expireTime, long senderId, string recepientEmail)
        {
            Id = id;
            BoxId = boxId;
            ExpireTime = expireTime;
            SenderId = senderId;
            RecepientEmail = recepientEmail;
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
        public string RecepientEmail { get; private set; }
    }
}
