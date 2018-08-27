using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class GoogleTokenRequest
    {
        [Required]
        public string Token { get; set; }

    }
}