using System;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoBuf.ProtoContract]
    public class FileProcessData
    {
        [ProtoBuf.ProtoMember(1)]
        public Uri BlobName { get; set; }

        [ProtoBuf.ProtoMember(2)]

        public long ItemId { get; set; }
    }
}
