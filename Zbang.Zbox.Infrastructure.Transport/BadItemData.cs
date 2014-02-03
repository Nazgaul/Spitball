using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class BadItemData : DomainProcess
    {
        protected BadItemData()
        {
        }
        public BadItemData(string reason, string other, long userId, long itemId)
        {
            Reason = reason;
            Other = other;
            UserId = userId;
            ItemId = itemId;
        }
        [ProtoMember(1)]
        public string Reason { get; private set; }

        [ProtoMember(2)]
        public string Other { get; private set; }

        [ProtoMember(3)]
        public long UserId { get; private set; }

        [ProtoMember(4)]
        public long ItemId { get; private set; }

        public override string ProcessResolver
        {
            get { return DomainProcess.BadItemResolver; }
        }
    }
}
