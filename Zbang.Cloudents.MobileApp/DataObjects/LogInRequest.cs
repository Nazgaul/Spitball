
using System.ComponentModel.DataAnnotations;

using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.MobileApp.DataObjects
{
    public class LogInRequest
    {
        [Required]//(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "EmailRequired")]
        [RegularExpression(Validation.EmailRegexWithTrailingEndingSpaces)]// ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "NotValidEmail")]
        public string Email { get; set; }

        [Required]//(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        public string Password { get; set; }
    }
}