using System;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public Uri Image { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Snippet { get; set; }
        [DataMember]
        public Uri Url { get; set; }

        [DataMember]
        public string Source { get; set; }

        public int Order { get; set; }
    }
}