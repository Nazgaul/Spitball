using System.ComponentModel.DataAnnotations;

namespace Cloudents.MobileApi.Models
{
    /// <summary>
    /// Course request object
    /// </summary>
    public class CourseRequest
    {
        /// <summary>
        /// User input
        /// </summary>
        [StringLength(int.MaxValue,MinimumLength = 3)]
        public string Term { get; set; }

        /// <summary>
        /// university of the user
        /// </summary>
        [Required]
        public long? UniversityId { get; set; }
    }
}