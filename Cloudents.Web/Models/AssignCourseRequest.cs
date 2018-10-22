using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class AssignCourseRequest
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
    }
}