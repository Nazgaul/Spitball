using System;
using System.Runtime.Serialization;
using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class DocumentFeedDto
    {
        public long Id { get; set; }
        [DataMember]
        public string University { get; set; }
        [DataMember]
        public string Course { get; set; }
        [DataMember]
        public string Snippet { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Professor { get; set; }
        //[DataMember]
        public DocumentType? TypeStr { get; set; }

        [DataMember]
        public string Type => TypeStr?.ToString("G");
        [DataMember]
        public UserDto User { get; set; }
        [DataMember]
        public int? Views { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }
    }
}