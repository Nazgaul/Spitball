using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetChildNameRequest
    {
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
      

        [Required]
        [Range(1, 12)]
        public short Grade { get; set; }
    }
}
