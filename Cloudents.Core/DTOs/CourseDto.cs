
namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        //public CourseDto(string name)
        //{
        //    Name = name;
        //}

        //public CourseDto(string name, bool? isFollowing, int students, bool? isPending)
        //{
        //    Name = name;
        //    IsFollowing = isFollowing;
        //    Students = students;
        //    IsPending = isPending;
        //}

        //public CourseDto(string name, ItemState state, int students)
        //{
        //    Name = name;
        //    Students = students;
        //    State = state;
        //}

        public string Name { get; private set; }
        public bool? IsFollowing { get; private set; }
        public int Students { get; private set; }
        public bool? IsPending { get; private set; }
        public bool? IsTeaching { get; private set; }
    }
    
}
