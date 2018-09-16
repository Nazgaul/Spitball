using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class GoogleTokenRequest
    {
        [Required]
        public string Token { get; set; }

    }
}