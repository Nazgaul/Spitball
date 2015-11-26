﻿using ProtoBuf;
using System;

namespace Zbang.Zbox.Infrastructure.Transport
{
    [ProtoContract]
    public class UrlToDownloadData
    {
        protected UrlToDownloadData() { }

        public UrlToDownloadData(string url, string fileName, long boxId,  long userId)
        {
            Url = url;
            FileName = fileName;
            BoxId = boxId;
            UserId = userId;
        }
        [ProtoMember(1)]
        public string Url { get; set; }
        [ProtoMember(2)]
        public string FileName { get; set; }
        [ProtoMember(3)]
        public long BoxId { get; set; }
       

        [ProtoMember(5)]
        public long UserId { get; set; }

        [ProtoMember(6)]
        public string BlobUrl { get; set; }

        [ProtoMember(7)]
        public long? Size { get; set; }
    }
}
