namespace Cloudents.Core.DTOs.SearchSync
{
    public class CourseSearchDto
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public long UniversityId { get; set; }
        public bool IsDeleted { get; set; }

        public long Id { get; set; }
    }
}