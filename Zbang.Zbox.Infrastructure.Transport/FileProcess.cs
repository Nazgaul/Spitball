using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoInclude(1, typeof(ChatFileProcessData))]
    [ProtoInclude(3, typeof(SignalrConnectionsData2))]
    [ProtoContract]
    public abstract class FileProcess
    {
        public abstract string ProcessResolver { get; }
    }
}