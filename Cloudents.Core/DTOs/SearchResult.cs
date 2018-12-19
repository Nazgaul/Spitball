using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cloudents.Application.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "dto class convert to json")]
    public class SearchResult : IShuffleable, IUrlRedirect
    {
        [DataMember]
        public string Id { get; set; }//id

        [DataMember]
        public Uri Image { get; set; }//image

        [DataMember]
        public string Title { get; set; }//name

        [DataMember]
        public string Snippet { get; set; }//metaContent

        [DataMember]
        public string Url { get; set; }//url

        [DataMember]
        public string Source { get; set; } //not relevant

        public PrioritySource PrioritySource { get; set; }//not relevant

        public int Order { get; set; }//not relevant
    }
}