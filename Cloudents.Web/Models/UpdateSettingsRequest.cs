using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class UpdateSettingsRequest
    {
        [Required]
        [StringLength(255, ErrorMessage = "StringLength")]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }
        [StringLength(255, ErrorMessage = "StringLength")]
        public string Description { get; set; }
    }

    public class BecomeTutorRequest
    {
        public string Bio { get; set; }
        public decimal Price { get; set; }
    }
}
