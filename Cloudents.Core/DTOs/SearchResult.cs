using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Cloudents.Core.DTOs
{
    [DataContract]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "dto class convert to json")]
    public class SearchResult : IShuffleable, IUrlRedirect
    {
        public string Id { get; set; }
        public Uri Image { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public string Url { get; set; }

        public string Source => PrioritySource.ToString();

        [IgnoreDataMember]
        public PrioritySource PrioritySource { get; set; }

        [IgnoreDataMember]
        public int Order { get; set; }
    }
}