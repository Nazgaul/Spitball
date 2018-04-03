using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "dto class convert to json")]
    public class SearchResult : IShuffleable, IUrlRedirect
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
        public string Url { get; set; }

        [DataMember]
        public string Source { get; set; }

        public PrioritySource PrioritySource { get; set; }

        //[DataMember]
        public int Order { get; set; }
    }
}