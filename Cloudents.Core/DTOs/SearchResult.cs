using System;

namespace Cloudents.Core.DTOs
{
    public class SearchResult
    {
        public string Id { get; set; }
        public Uri Image { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public Uri Url { get; set; }

        public string Source { get; set; }
    }
}