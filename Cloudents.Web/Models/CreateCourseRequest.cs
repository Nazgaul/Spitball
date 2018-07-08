using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateCourseRequest
    {
        /// <summary>
        /// The course name
        /// </summary>
        [Required]
        [StringLength(120)]
        public string CourseName { get; set; }

        /// <summary>
        /// User university
        /// </summary>
        [Required]
        [Range(1,long.MaxValue)]
        public long University { get; set; }
    }
}
