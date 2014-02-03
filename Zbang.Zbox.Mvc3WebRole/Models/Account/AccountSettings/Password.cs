using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
{
    public class Password
    {
        [DataType(DataType.Password)]
        [RegularExpression(".{6,}", ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [ValidatePasswordLengthAttribute(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "CurrentPassword")]
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "CurrentPwdEmpty")]
        public string CurrentPassword { get; set; }


        [DataType(DataType.Password)]
        [RegularExpression(".{6,}", ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [ValidatePasswordLengthAttribute(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "Min6Chars")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "NewPassword")]
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "NewPwdEmpty")]
        public string NewPassword { get; set; }
    }
}