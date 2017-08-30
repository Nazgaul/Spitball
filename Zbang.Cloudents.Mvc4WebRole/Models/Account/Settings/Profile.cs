using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Profile 
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "UsernameEmpty")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "LastNameRequired")]
        public string LastName { get; set; }
    }
}