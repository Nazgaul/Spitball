using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoInclude(1, typeof(ChatFileProcessData))]
    [ProtoContract]
    public abstract class FileProcess
    {
        public abstract string ProcessResolver { get; }
    }
}