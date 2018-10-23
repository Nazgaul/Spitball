using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

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
        [FromQuery]
        public string Term { get; set; }
    }
}