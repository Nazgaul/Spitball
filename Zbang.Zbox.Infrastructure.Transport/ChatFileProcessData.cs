using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class ChatFileProcessData : FileProcess
    {
        protected ChatFileProcessData()
        {
            
        }

        public ChatFileProcessData(Uri blobUri, IList<long> users)
        {
            BlobUri = blobUri;
            Users = users;
        }
        [ProtoMember(1)]
        public Uri BlobUri { get; private set; }

        [ProtoMember(2)]
        public IList<long> Users { get; private set; }
        public override string ProcessResolver => nameof(ChatFileProcessData);
    }

    [ProtoContract]
    public class SignalrConnectionsData : FileProcess
    {
        protected SignalrConnectionsData()
        {
        }

        public SignalrConnectionsData(IEnumerable<string> connectionIds)
        {
            ConnectionIds = connectionIds;
        }

        [ProtoMember(1)]
        public IEnumerable<string> ConnectionIds { get; private set; }
        public override string ProcessResolver => nameof(SignalrConnectionsData);
    }
}
