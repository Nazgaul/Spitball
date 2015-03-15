using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class PasswordUpdateRequest
    {
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string ResetToken { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}