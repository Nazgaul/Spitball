using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Password
    {
        [DataType(DataType.Password)]
        [RegularExpression(".{6,}", ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [ValidatePasswordLength(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "CurrentPassword")]
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "CurrentPwdEmpty")]
        public string CurrentPassword { get; set; }


        [DataType(DataType.Password)]
        [RegularExpression(".{6,}", ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [ValidatePasswordLength(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "NewPassword")]
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "NewPwdEmpty")]
        public string NewPassword { get; set; }
    }
}