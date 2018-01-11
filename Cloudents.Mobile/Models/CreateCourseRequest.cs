using System.ComponentModel.DataAnnotations;

namespace Cloudents.Mobile.Models
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
        [StringLength(Zbang.Zbox.Domain.Box.NameLength)]
        public string CourseName { get; set; }

        /// <summary>
        /// User university
        /// </summary>
        [Required]
        public long? University { get; set; }
    }
}