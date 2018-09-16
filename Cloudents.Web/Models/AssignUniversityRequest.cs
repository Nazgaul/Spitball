using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class AssignUniversityRequest
    {
        [Required(ErrorMessage = "Required")]
        [Range(1,long.MaxValue)]
        public long UniversityId { get; set; }
    }
}
