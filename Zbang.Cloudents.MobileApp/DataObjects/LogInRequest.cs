
using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class LogInRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public override string ToString()
        {
            return String.Format("Email: {0} password: {1}", Email, Password);
        }
    }

    public class ExternalLoginRequest
    {
        [Required]
        public string AuthToken { get; set; }
    }
}