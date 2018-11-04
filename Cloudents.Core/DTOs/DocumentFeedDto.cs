using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class DocumentFeedDto
    {
        public long Id { get; set; }
        public string University { get; set; }
        public string Course { get; set; }
        public string Snippet { get; set; }
        public string Title { get; set; }
        public string Professor { get; set; }
        public DocumentType DocumentType { get; set; }
        public UserDto User { get; set; }
        public int Views { get; set; }
    }
}