using System;
using System.ComponentModel.DataAnnotations;
using Zbang.Cloudents.Mobile.Models.Resources;

namespace Zbang.Cloudents.Mvc4WebRole.Models
{
    public class CreateAcademicBox 
    {

        [DataType(DataType.Text)]
        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "CourseName")]
        [StringLength(Zbox.Domain.Box.NameLength, ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "BoxNameUpTo")]
        public string CourseName { get; set; }

        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "CourseCode")]
        public string CourseId { get; set; }

        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "Instructor")]
        public string Professor { get; set; }

        //[Obsolete]
        //[Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "Department")]
        //public string Department { get; set; }

        [Required(ErrorMessageResourceType = typeof(CreateBoxResources), ErrorMessageResourceName = "FieldRequired")]
        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "Department")]
        public string DepartmentId { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            return string.Format("{0} CourseName {1} CourseId {2} Professor {3} DepartmentId {4}", baseString, CourseName, CourseId, Professor, DepartmentId);

        }
    }
}