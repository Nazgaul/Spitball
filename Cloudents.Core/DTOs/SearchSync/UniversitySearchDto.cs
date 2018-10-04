
namespace Cloudents.Core.DTOs.SearchSync
{
    public class UniversitySearchDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string Extra { get; set; }

        public string Image { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

        public string Country { get; set; }
    }
}