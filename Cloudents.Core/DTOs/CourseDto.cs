using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public class CourseDtoEquality : IEqualityComparer<CourseDto>
    {
        public bool Equals(CourseDto x, CourseDto y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode(CourseDto obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
