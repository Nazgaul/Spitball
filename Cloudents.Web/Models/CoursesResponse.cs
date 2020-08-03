using Cloudents.Core.DTOs;
using System.Collections.Generic;

namespace Cloudents.Web.Models
{
    public class CoursesResponse
    {
        public CoursesResponse(IEnumerable<CourseNameDto> courses)
        {
            Courses = courses;
        }

        public IEnumerable<CourseNameDto> Courses { get; set; }
    }
}