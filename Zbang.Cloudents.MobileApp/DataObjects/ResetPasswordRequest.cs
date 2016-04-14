using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}