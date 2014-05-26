using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account
{
    public class Confirmation
    {
        public string Key { get; set; }
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "CodeIncorrect2")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Code")]
        public string Code { get; set; }
    }
}