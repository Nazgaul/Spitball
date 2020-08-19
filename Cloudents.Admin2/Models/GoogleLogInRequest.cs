using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Cloudents.Admin2.Models
{
    public class GoogleLogInRequest
    {
        [Required]
        public string Token { get; set; } = null!;
    }
}
