using System.Collections.Generic;
using Cloudents.Core.DTOs;

namespace Cloudents.Web.Models
{
    public class CoursesResponse
    {
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}