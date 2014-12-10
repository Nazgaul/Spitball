using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Filters;
using Zbang.Cloudents.Mobile.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class NewPassword
    {
        [Required(ErrorMessageResourceType = typeof(LogOnResources), ErrorMessageResourceName = "PwdAtLeast6Chars")]
        [ValidatePasswordLength(ErrorMessageResourceName = "PwdAtLeast6Chars", ErrorMessageResourceType = typeof(LogOnResources))]
        [DataType(DataType.Password)]
        [Display(ResourceType = typeof(LogOnResources), Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "PwdRequired")]
        [Compare("Password", ErrorMessageResourceType = typeof(RegisterResources), ErrorMessageResourceName = "ConfirmPasswordComapre")]
        [Display(ResourceType = typeof(RegisterResources), Name = "ConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}