namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        public CourseDto(string name)
        {
            Name = name;
        }

        public CourseDto(string name, bool isFollowing, int students)
        {
            Name = name;
            IsFollowing = isFollowing;
            Students = students;
        }

        public string Name { get;  }
        public bool? IsFollowing { get; set; }
        public int Students { get; set; }
    }
    
}
