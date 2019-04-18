
namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        public string Name { get; private set; }
        public bool? IsFollowing { get; private set; }
        public int Students { get; private set; }
        public bool? IsPending { get; private set; }
        public bool? IsTeaching { get; private set; }
    }
    
}
