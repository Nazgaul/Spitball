using System;

namespace Cloudents.Core.DTOs
{
    public class SearchResult
    {
        public string Id { get; set; }
        public Uri Image { get; set; }
        public int? Views { get; set; }
        public string University { get; set; }
        public string Course { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public string Url { get; set; }

        public string Source { get; set; }
    }
}