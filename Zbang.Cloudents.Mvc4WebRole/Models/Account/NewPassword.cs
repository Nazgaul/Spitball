using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class NewPassword
    {
        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        [ValidatePasswordLength(ErrorMessageResourceName = "PwdAtLeast6Chars", ErrorMessageResourceType = typeof(LogOnResources))]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [Compare("Password", ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "ConfirmPasswordCompare")]
        public string ConfirmPassword { get; set; }
    }
}