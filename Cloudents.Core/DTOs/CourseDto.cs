using Cloudents.Core.Enum;

namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        public CourseDto(string name)
        {
            Name = name;
        }

        public CourseDto(string name, bool? isFollowing, int students, ItemState? state)
        {
            Name = name;
            IsFollowing = isFollowing;
            Students = students;
            State = state;
        }

        //public CourseDto(string name, ItemState state, int students)
        //{
        //    Name = name;
        //    Students = students;
        //    State = state;
        //}

        public string Name { get;  }
        public bool? IsFollowing { get; set; }
        public int Students { get; set; }
        public ItemState? State { get; set; }
    }
    
}
