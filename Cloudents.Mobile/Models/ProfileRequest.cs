using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class ProfileRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string LastName { get; set; }
    }
}