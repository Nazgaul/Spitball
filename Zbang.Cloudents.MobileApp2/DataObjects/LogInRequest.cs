
using System;
using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.MobileApp2.DataObjects
{
    public class LogInRequest
    {
        [Required]//(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "EmailRequired")]
        //[RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces)]// ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail")]
        public string Email { get; set; }

        [Required]//(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
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