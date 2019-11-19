
using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class UpdateNameRequest
    {
        [Required]
        public long UserId { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string LastName { get; set; }
    }
}
