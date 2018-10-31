using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class AssignCoursesRequest
    {
        public AssignCourseRequest[] Courses { get; set; }
    }
    public class AssignCourseRequest
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(150, MinimumLength = 4, ErrorMessage = "StringLength")]
        public string Name { get; set; }
    }
}