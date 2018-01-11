using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class CreateUniversityRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(2)]
        public string Country { get; set; }
    }
}