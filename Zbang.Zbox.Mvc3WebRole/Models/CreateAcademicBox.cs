using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Zbang.Zbox.Mvc3WebRole.Models.Resources;

namespace Zbang.Zbox.Mvc3WebRole.Models
{
    public class CreateAcademicBox : CreateBox
    {
        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "CourseName")]
        public string CourseName { get; set; }

        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "CourseCode")]
        public string CourseId { get; set; }

        [DataType(DataType.Text)]
        [Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "Instructor")]
        public string Professor { get; set; }

        //[Obsolete]
        //[Display(ResourceType = typeof(CreateAcademicBoxResources), Name = "Department")]
        //public string Department { get; set; }

        [Required]
        public Guid ParentId { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            return string.Format("{0} CourseName {1} CourseId {2} Professor {3} ParentId {4}", baseString, CourseName, CourseId, Professor, ParentId);

        }
    }
}