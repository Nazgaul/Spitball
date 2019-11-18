using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class CoursesResponse
    {
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}