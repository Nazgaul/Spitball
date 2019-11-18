using System.ComponentModel.DataAnnotations;

namespace Cloudents.Web.Models
{
    public class CreateQuestionRequest
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Required")]
        [StringLength(8000, MinimumLength = 15, ErrorMessage = "MinLength")]
        public string Text { get; set; }


        [Required]
        public string Course { get; set; }
    }
}

