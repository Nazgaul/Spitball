using Cloudents.Web.Identity;
using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Swagger;
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
        [StringLength(int.MaxValue, MinimumLength = 3, ErrorMessage = "StringLength")]
        [Required(ErrorMessage = "Required")]
        [FromQuery]
        public string Term { get; set; }
    }
}