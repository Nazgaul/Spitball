
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Core.DTOs
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local", Justification = "Reflection")]
    public class CourseDto
    {
        public string Name { get; private set; }
        public bool? IsFollowing { get; private set; }
        public int Students { get; private set; }
       // public bool? IsPending { get; private set; }
       // public bool? IsTeaching { get; private set; }
    }


    public class UserCourseDto
    {
        public string Name { get;  set; }
       // public bool? IsFollowing { get;  set; }
        public int Students { get;  set; }
        public bool? IsPending { get;  set; }
        public bool? IsTeaching { get;  set; }
    }

}
