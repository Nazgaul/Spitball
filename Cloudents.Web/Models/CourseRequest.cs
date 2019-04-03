using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// Course request object
    /// </summary>
    public class CourseRequest
    {
        /// <summary>
        /// User input
        /// </summary>
        [StringLength(150, MinimumLength = 3, ErrorMessage = "StringLength")]
        [Required(ErrorMessage = "Required")]
        public string Term { get; set; }
    }
}