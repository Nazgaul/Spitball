using System.ComponentModel.DataAnnotations;
using Cloudents.Web.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cloudents.Web.Models
{
    //TODO:Localize
    public class CreateCourseRequest
    {
        /// <summary>
        /// The course name
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [StringLength(120)]
        public string CourseName { get; set; }

       
    }
}
