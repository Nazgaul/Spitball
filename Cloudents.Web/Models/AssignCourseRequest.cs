using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class SetCourseRequest
    {
        [Required(ErrorMessage = "Required")]
        //[RegularExpression(@"(?=\p{L}|[0-9]).{4,100}$")]
        public string Name { get; set; }
    }


}