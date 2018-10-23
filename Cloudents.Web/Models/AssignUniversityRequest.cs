using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class AssignUniversityRequest
    {
        [StringLength(100, MinimumLength = 10, ErrorMessage = "StringLength")]

        [Required(ErrorMessage = "Required")]
        public string Name { get; set; }
    }
}
