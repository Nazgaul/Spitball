using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetCourseRequest
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(150, MinimumLength = 4, ErrorMessage = "StringLength")]
        public string Name { get; set; }
    }
}