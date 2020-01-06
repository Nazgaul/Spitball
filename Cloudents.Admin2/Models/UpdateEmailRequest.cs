using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UpdateEmailRequest
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
