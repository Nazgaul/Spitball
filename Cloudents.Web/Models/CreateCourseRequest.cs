using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateCourseRequest
    {
        /// <summary>
        /// The course name
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [StringLength(120,ErrorMessage = "StringLengthMax")]
        public string CourseName { get; set; }

       
    }
}
