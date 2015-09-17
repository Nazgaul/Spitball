﻿using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class Profile 
    {
        //readonly fields
        //[Display(ResourceType = typeof(AccountSettingsResources), Name = "School")]
        public long UniversityId { get; set; }


        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "UsernameEmpty")]
        [Display(ResourceType = typeof(AccountSettingsResources), Name = "Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "LastNameRequired")]
        public string LastName { get; set; }
    }
}