using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
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