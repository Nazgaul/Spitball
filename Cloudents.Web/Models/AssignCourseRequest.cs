using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetCourseRequest
    {
        [Required(ErrorMessage = "Required")]
        //[StringLength(150, MinimumLength = 4, ErrorMessage = "StringLength")]
        [RegularExpression(@"(?=\p{L}|[0-9]).{4,100}$")]
        public string Name { get; set; }
    }
}