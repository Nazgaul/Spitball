using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
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

        public Password Password { get; set; }

        public ChangeMail NewEmail { get; set; }
    }
}