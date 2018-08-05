using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
    }
}