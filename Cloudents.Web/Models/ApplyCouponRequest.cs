using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Cloudents.Web.Models
{
    public class ApplyCouponRequest
    {
        [Required]
        public string Coupon { get; set; }

        [Required] 
        public long TutorId { get; set; }

        [Required]
        [JsonProperty("RoomId")]
        public long CourseId { get; set; }

    }
}