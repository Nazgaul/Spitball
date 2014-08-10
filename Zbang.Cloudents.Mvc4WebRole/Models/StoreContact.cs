using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zbang.Cloudents.Mvc4WebRole.Views.Shared.Resources; //TODO: move localization to Model
using Zbang.Cloudents.Mvc4WebRole.Views.Store.Resources;
namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class StoreContact
    {
        [Required(ErrorMessageResourceType = typeof(DialogResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(SharedResources), Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(DialogResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(StoreResources), Name = "Telephone")]
        public string Phone { get; set; }

        [Display(ResourceType = typeof(SharedResources), Name = "School")]
        public string University { get; set; }

        [Required(ErrorMessageResourceType = typeof(DialogResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(SharedResources), Name = "Email")]
        public string Email { get; set; }
        

        [Required(ErrorMessageResourceType = typeof(DialogResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(SharedResources), Name = "Message")]
        public string Text { get; set; }
    }
}