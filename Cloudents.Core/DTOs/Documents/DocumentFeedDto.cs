using Cloudents.Core.Attributes;
using Cloudents.Core.Entities;
using Cloudents.Core.Enum;
using System;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentFeedDto //: FeedDto
    {
        [EntityBind(nameof(Document.Id))]
        public long Id { get; set; }
        public string Title { get; set; }

        public string Preview { get; set; }

        public DocumentType DocumentType { get; set; }

        public string Course { get; set; }
    }

    
}