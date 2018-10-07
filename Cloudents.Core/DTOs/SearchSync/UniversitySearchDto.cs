
namespace Cloudents.Core.DTOs.SearchSync
{
    public class UniversitySearchDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Extra { get; set; }

        public string Country { get; set; }

        public bool Pending { get; set; }
    }
}