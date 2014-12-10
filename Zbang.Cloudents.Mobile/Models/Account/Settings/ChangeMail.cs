using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Zbang.Cloudents.Mobile.Models.Account.Resources;
using Zbang.Zbox.Domain.Common;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
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