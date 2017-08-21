using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Password
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "CurrentPwdEmpty")]
        [MinLength(6,ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        public string CurrentPassword { get; set; }

        [MinLength(6, ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "NewPwdEmpty")]
        public string NewPassword { get; set; }
    }
}