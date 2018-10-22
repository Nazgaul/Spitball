using System;
using System.Collections.Generic;

namespace Cloudents.Core.DTOs
{
    public class CourseDto
    {
        //public long Id { get; set; }
        public string Name { get; set; }
    }

    public class CourseDtoEquality : IEqualityComparer<CourseDto>
    {
        public bool Equals(CourseDto x, CourseDto y)
        {
            return string.Equals(x?.Name, y?.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(CourseDto obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
