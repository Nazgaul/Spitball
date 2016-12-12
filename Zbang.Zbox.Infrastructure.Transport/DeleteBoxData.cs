
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class DeleteBoxData : DomainProcess
    {
        protected DeleteBoxData()
        {
            
        }
        public DeleteBoxData(long boxId)
        {
            BoxId = boxId;
        }
        [ProtoMember(1)]
        public long BoxId { get; }

        public override string ProcessResolver => DeleteBoxResolver;

        public override string ToString()
        {
            return "BoxId: " + BoxId;
        }
    }
}
