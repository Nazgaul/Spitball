using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class AssignUniversityRequest
    {
        [Required(ErrorMessage = "Required")]
        [Range(1,long.MaxValue,ErrorMessage = "Range")]
        public long UniversityId { get; set; }
    }
}
