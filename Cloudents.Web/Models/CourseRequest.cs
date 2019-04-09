using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Framework;

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
        [RequiredPropertyForQuery]
        public string Term { get; set; }
    }
}