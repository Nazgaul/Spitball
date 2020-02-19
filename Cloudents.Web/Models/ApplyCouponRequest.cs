using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ApplyCouponRequest
    {
        [Required]
        public string Coupon { get; set; }

        [Required] 
        public long TutorId { get; set; }

    }
}