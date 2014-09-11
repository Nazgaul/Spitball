using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Profile 
    {
       

        //readonly fields
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "School")]
        public string University { get; set; }
        public string UniversityImage { get; set; }

        //TODO add validation to Image url
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Photo")]
        public string Image { get; set; }

        //TODO add validation to Image url
        public string LargeImage { get; set; }



        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "UsernameEmpty")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "LastNameRequired")]
        public string LastName { get; set; }
    }
}