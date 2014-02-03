using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Zbox.Domain.Common;
using Zbang.Zbox.Mvc3WebRole.Models.Account.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models.Account.AccountSettings
{
    [Bind(Exclude = "Code")]
    [System.Serializable]
    public class ChangeMail
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailEmpty")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(Validation.EmailRegex, ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "EmailNotCorrect")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Email")]
        public string Email { get; set; }


        [Display(ResourceType = typeof(AccountSettingsResources), Name = "TypeCodeReceived")]
        public int? Code { get; set; }

    }
}