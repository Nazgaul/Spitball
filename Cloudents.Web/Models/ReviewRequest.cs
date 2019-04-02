using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ReviewRequest
    {
        [StringLength(1000)]
        [Required(ErrorMessage = "Required")]
        public string Review { get; set; }
        [Range(0, 5)]
        [Required(ErrorMessage = "Required")]
        public float Rate { get; set; }
        [Required(ErrorMessage = "Required")]
        public long Tutor { get; set; }
    }
}
