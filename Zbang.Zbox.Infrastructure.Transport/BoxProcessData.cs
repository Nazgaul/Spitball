using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class BoxProcessData : FileProcess
    {
        protected BoxProcessData()
        {
        }

        public BoxProcessData(long boxId)
        {
            BoxId = boxId;
        }

        [ProtoMember(1)]
        public long BoxId { get; private set; }
        public override string ProcessResolver => nameof(BoxProcessData);
    }

    public class BoxDeleteData : FileProcess
    {
        protected BoxDeleteData()
        {
        }

        public BoxDeleteData(long boxId)
        {
            BoxId = boxId;
        }

        [ProtoMember(1)]
        public long BoxId { get; private set; }
        public override string ProcessResolver => nameof(BoxDeleteData);
    }


}