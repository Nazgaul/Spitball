using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetChildNameRequest
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(255)]
        public string lastName { get; set; }
    }
}
