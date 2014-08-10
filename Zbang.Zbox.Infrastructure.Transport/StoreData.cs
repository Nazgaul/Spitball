using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    [ProtoInclude(10, typeof (StoreOrderData))]
    [ProtoInclude(11, typeof (StoreContactData))]
    public abstract class StoreData
    {
        
    }
}