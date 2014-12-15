using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ReputationData : DomainProcess
    {
        protected ReputationData()
        {
        }

        
        public ReputationData(long userId)
        {
            UserId = userId;
        }

        [ProtoMember(1)]
        public long UserId { get; private set; }
        public override string ProcessResolver
        {
            get { return ReputationResolver; }
        }
    }
}
