using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoInclude(1, typeof(ChatFileProcessData))]
    [ProtoInclude(4, typeof(BoxFileProcessData))]
    [ProtoContract]
    public abstract class FileProcess
    {
        public abstract string ProcessResolver { get; }
    }
}