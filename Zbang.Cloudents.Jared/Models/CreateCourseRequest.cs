using System.ComponentModel.DataAnnotations;

namespace Zbang.Cloudents.Jared.Models
{
    /// <summary>
    /// Create course object
    /// </summary>
    public class CreateCourseRequest
    {
        /// <summary>
        /// The course name
        /// </summary>
        [Required]
        [StringLength(Zbox.Domain.Box.NameLength)]
        public string CourseName { get; set; }

        /// <summary>
        /// The course code 
        /// </summary>
        public string CourseId { get; set; }

        //public string Professor { get; set; }

        //[Required]
        //public string DepartmentId { get; set; }

        public override string ToString()
        {
            return $"CourseName {CourseName} CourseId {CourseId}";
        }
    }
}