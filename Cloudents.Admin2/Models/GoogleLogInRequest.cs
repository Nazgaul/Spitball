using System.ComponentModel.DataAnnotations;

namespace Cloudents.Admin2.Models
{
    public class GoogleLogInRequest
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
