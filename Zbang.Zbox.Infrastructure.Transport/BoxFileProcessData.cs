using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class BoxFileProcessData : FileProcess
    {
        protected BoxFileProcessData()
        {
        }

        public BoxFileProcessData(long itemId)
        {
            ItemId = itemId;
        }

        [ProtoMember(1)]
        public long ItemId { get; private set; }
        public override string ProcessResolver => nameof(BoxFileProcessData);

        public override string ToString()
        {
            return "itemId " + ItemId;
        }
    }
}