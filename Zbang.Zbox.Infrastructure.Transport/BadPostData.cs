using System;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class BadPostData : DomainProcess
    {
        protected BadPostData()
        {
        }
        public BadPostData(long userId, Guid postId)
        {
            UserId = userId;
            PostId = postId;
        }
      

        [ProtoMember(1)]
        public long UserId { get; private set; }

        [ProtoMember(2)]
        public Guid PostId { get; private set; }

        public override string ProcessResolver => BadItemResolver;
    }
}