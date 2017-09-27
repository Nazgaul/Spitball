using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    public class CreateAcademicCourseRequest
    {
        [Required]
        [StringLength(Zbox.Domain.Box.NameLength)]
        public string CourseName { get; set; }

        public string CourseId { get; set; }

        //public string Professor { get; set; }

        //[Required]
        //public string DepartmentId { get; set; }

        public override string ToString()
        {
            var baseString = base.ToString();
            return $"{baseString} CourseName {CourseName} CourseId {CourseId}";
        }
    }
}