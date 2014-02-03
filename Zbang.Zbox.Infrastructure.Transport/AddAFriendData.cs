using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class AddAFriendData : DomainProcess
    {
        protected AddAFriendData()
        {
        }
        public AddAFriendData(long userId, long friendId)
        {
            UserId = userId;
            FriendId = friendId;
        }
        [ProtoMember(1)]
        public long UserId { get; private set; }

        [ProtoMember(2)]
        public long FriendId { get; private set; }

        public override string ProcessResolver
        {
            get { return DomainProcess.AddAFriendResolver; }
        }
    }
}
