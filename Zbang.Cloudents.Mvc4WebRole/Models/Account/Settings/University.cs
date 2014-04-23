using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class University
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "SchoolRequired")]
        public long UniversityId { get; set; }

        [Display(ResourceType = typeof(AccountSettingsResources), Name = "IHaveCode")]
        public string Code { get; set; }

        public long? DepartmentId { get; set; }

        public string GroupNumber { get; set; }
        public string RegisterNumber { get; set; }

        public string studentID { get; set; }
    }
}