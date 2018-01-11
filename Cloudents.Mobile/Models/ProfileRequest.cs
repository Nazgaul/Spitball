using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
{
    public class ProfileRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string LastName { get; set; }
    }
}