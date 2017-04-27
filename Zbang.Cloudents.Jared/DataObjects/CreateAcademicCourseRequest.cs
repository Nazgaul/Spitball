using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Zbang.Cloudents.Jared.DataObjects
{
    public class CreateAcademicCourseRequest 
    {
        [Required]
        [StringLength(Zbox.Domain.Box.NameLength)]
        public string CourseName { get; set; }

        public string CourseId { get; set; }

        public string Professor { get; set; }


        //[Required]
        //public string DepartmentId { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            return $"{baseString} CourseName {CourseName} CourseId {CourseId} Professor {Professor}";

        }
    }
}