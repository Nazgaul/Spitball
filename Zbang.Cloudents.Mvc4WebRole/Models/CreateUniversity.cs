using Zbang.Zbox.Infrastructure.Enums;
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateUniversity
    {

        [Required(ErrorMessageResourceType = typeof(CreateUniversityResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateUniversityResources), Name = "SchoolName")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateUniversityResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateUniversityResources), Name = "Country")]
        public string Country { get; set; }

        [Display(ResourceType = typeof(CreateUniversityResources), Name = "Type")]
        public SchoolType SchoolType { get; set; }
    }
}