using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    /// <summary>
    /// University request object
    /// </summary>
    public class UniversityRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        [StringLength(100, MinimumLength = 2, ErrorMessage = "StringLength")]
        public string Term { get; set; }

        public int Page { get; set; }
    }


    /// <summary>
    /// University request object
    /// </summary>
    public class CourseSearchRequest
    {
        /// <summary>
        /// the user input
        /// </summary>
        [StringLength(100, MinimumLength = 3, ErrorMessage = "StringLength")]
        public string Term { get; set; }

        public int Page { get; set; }
    }
}