using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Account.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models.Account.Settings
{
    public class University
    {
        [Required(ErrorMessageResourceType = typeof(AccountSettingsResources), ErrorMessageResourceName = "SchoolRequired")]
        public long UniversityId { get; set; }


        public long? DepartmentId { get; set; }

        public string GroupNumber { get; set; }
        public string RegisterNumber { get; set; }

        public string studentID { get; set; }

        public override string ToString()
        {
            return string.Format(
                "University id {0} DepartmentId {1} GroupNumber {2} RegisterNumber {3} studentID {4}",
                UniversityId,  DepartmentId, GroupNumber, RegisterNumber, studentID);
            
        }
    }
}