using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateUniversity
    {
        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        //[Display(ResourceType = typeof(CreateUniversityResources), Name = "SchoolName")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        //[Display(ResourceType = typeof(CreateUniversityResources), Name = "Country")]
        [MaxLength(2)]
        public string Country { get; set; }
    }
}