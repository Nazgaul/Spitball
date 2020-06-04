using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class GoogleTokenRequest
    {
        [Required(ErrorMessage = "Required")]
        public string Token { get; set; }

        public UserType UserType { get; set; }

    }
}