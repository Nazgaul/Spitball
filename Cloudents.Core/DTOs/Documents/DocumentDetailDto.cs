using Cloudents.Core.DTOs.Tutors;

namespace Cloudents.Core.DTOs.Documents
{
    public class DocumentDetailDto
    {
        public TutorCardDto? Tutor { get; set; }
        public DocumentFeedDto Document{ get; set; }
        public int Pages { get; set; }
        public bool IsPurchased { get; set; }

        public long? DuplicateId { get; set; }

        public bool ShouldSerializeDuplicateId() => false;
    }

   
}
