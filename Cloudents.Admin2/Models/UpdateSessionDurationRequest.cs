using System;
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UpdateSessionDurationRequest
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public int Minutes { get; set; }
    }
}
