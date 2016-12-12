using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoInclude(1, typeof(ChatFileProcessData))]
    [ProtoInclude(4, typeof(BoxFileProcessData))]
    [ProtoInclude(5, typeof(UniversityProcessData))]
    [ProtoInclude(6, typeof(BoxProcessData))]
    [ProtoInclude(7, typeof(BoxDeleteData))]
    [ProtoContract]
    public abstract class FileProcess
    {
        public abstract string ProcessResolver { get; }
    }
}