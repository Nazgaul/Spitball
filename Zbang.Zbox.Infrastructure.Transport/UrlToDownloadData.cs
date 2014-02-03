using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class UrlToDownloadData
    {
        protected UrlToDownloadData() { }

        public UrlToDownloadData(string url, string fileName, long boxId, Guid? tabId, long userId)
        {
            Url = url;
            FileName = fileName;
            BoxId = boxId;
            TablId = tabId;
            UserId = userId;
        }
        [ProtoMember(1)]
        public string Url { get; set; }
        [ProtoMember(2)]
        public string FileName { get; set; }
        [ProtoMember(3)]
        public long BoxId { get; set; }
        [ProtoMember(4)]
        public Guid? TablId { get; set; }

        [ProtoMember(5)]
        public long UserId { get; set; }

        [ProtoMember(6)]
        public string BlobUrl { get; set; }

        [ProtoMember(7)]
        public long? Size { get; set; }
    }
}
