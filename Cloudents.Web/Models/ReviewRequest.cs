using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class ReviewRequest
    {
        [Required]
        public Guid RoomId { get; set; }
        [StringLength(1000)]
        [Required(ErrorMessage = "Required")]
        public string Review { get; set; }
        [Range(1, 5)]
        [Required(ErrorMessage = "Required")]
        public float Rate { get; set; }
       
    }
}
