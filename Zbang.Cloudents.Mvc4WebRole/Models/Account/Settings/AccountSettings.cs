using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class AccountSettings
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailEmpty")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Language")]
        public UserLanguage Language { get; set; }

        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Password")]
        public Password Password { get; set; }

        public ChangeMail NewEmail { get; set; }
    }
}