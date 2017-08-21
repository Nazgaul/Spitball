using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mvc4WebRole.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateAcademicBox
    {
        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        [StringLength(Zbox.Domain.Box.NameLength, ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameUpTo")]
        public string CourseName { get; set; }

        public string CourseId { get; set; }

        public string Professor { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        public string DepartmentId { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            return string.Format("{0} CourseName {1} CourseId {2} Professor {3} DepartmentId {4}", baseString, CourseName, CourseId, Professor, DepartmentId);
        }
    }
}