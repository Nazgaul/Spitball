using System;

namespace Cloudents.Core.Entities.Search
{
    public class Document
    {
        public string Id { get; set; }

        public Uri Image { get; set; }
        public string Name { get; set; }
        public string MetaContent { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public string UniversityId { get; set; }
        public long BoxId2 { get; set; }
    }
}