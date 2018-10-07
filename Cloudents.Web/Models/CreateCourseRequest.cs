using System.ComponentModel.DataAnnotations;

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

        ///TODO: REMOVE THIS
        /// <summary>
        /// User university
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [Range(1,long.MaxValue)]
        public long University { get; set; }
    }
}
