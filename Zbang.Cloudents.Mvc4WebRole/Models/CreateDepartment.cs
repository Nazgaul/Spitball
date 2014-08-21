
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;
using Zbang.Zbox.Infrastructure.Enums;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateDepartment
    {
        [Required(ErrorMessageResourceType = typeof(CreateUniversityResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateUniversityResources), Name = "CreateDept")]
        public string Name { get; set; }

    }
}