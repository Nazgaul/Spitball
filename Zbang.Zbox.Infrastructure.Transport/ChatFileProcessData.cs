using System;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ChatFileProcessData : FileProcess
    {
        protected ChatFileProcessData()
        {
            
        }

        public ChatFileProcessData(Uri blobUri)
        {
            BlobUri = blobUri;
        }
        [ProtoMember(1)]
        public Uri BlobUri { get; private set; }
        public override string ProcessResolver => nameof(ChatFileProcessData);
    }
}
