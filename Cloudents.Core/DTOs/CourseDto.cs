namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        public CourseDto(string name)
        {
            Name = name;
        }

        public CourseDto(string name, bool isRegistered, int users)
        {
            Name = name;
            IsRegistered = isRegistered;
            Users = users;
        }

        public string Name { get;  }
        public bool? IsRegistered { get; set; }
        public int Users { get; set; }
    }
    
}
