using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class CreateSubjectRequest
    {
        [StringLength(300)]
        [Required]
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }
      
    }
}
