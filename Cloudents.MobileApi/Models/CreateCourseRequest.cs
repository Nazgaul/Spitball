using System.ComponentModel.DataAnnotations;

namespace Cloudents.Api.Models
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
        [StringLength(120)]
        public string CourseName { get; set; }

        /// <summary>
        /// User university
        /// </summary>
        [Required]
        public long? University { get; set; }
    }
}