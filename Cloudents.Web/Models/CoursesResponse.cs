using System.Collections.Generic;
using Cloudents.Application.DTOs;

namespace Cloudents.Web.Models
{
    public class CoursesResponse
    {
        public IEnumerable<CourseDto> Courses { get; set; }
    }
}